using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class NPCMover : CharacterMover
{
    [Header("Waypoint Variables")]
    [SerializeField] private WaypointPath _initialPath = null;      // Mainly used when NPC is already in the level and not spawned.
    [SerializeField] private float _distanceThreshold = 0.5f;       // Destination is considered reached when distance to it is <= this.
    [SerializeField, Range(0f, 1f)] private float _laneAlignmentBias = 0f;  // Aligns agent to stay on a side of the lane more.
    [Header("Obstacle Avoidance")]
    [SerializeField] private LayerMask _steeringLayerMask;
    //[SerializeField] private float _speed = 1f;
    [SerializeField] private float _targetDistanceThreshold = 0.5f;
    //[SerializeField] private float _rotationSpeed = 180f;
    [SerializeField] private float _avoidanceCastLength = 1f;
    [SerializeField] private float _avoidanceRadius = 1f;
    [SerializeField] private float _avoidanceRadiusPadding = 0.1f;        // For cases where an agent keeps rotating away then towards an object next to it. Commonly occurs when 2 agents are next to each other.
    [Header("Unstucking Variables")]
    [SerializeField] private bool _enableUnstuck = true;            // Must be turned off for static/immovable agents.
    [SerializeField] private bool _enableUnstuckPosition = true;
    [SerializeField] private float _unstuckTimerLength = 0.5f;       // When position is within _unstuckMinDistance of _lastPosition, timer will start. Unstucking starts when timer hits 0.
    [SerializeField] private float _unstuckPositionMinDistance = 0.2f;      // When distance from _lastPosition is greater than this, _lastPosition will be reset.
    [SerializeField, Range(0, 180f)] private float _unstuckObstacleMinAngleThreshold = 80f;      // Mostly used for cases where agents get stuck together.
    [SerializeField, Range(0f, 180f)] private float _unstuckAngleRotationDifference = 90f;      // MUST BE HIGHER THAN MIN ANGLE THRESHOLD. Mostly used for cases where agents get stuck together. Unlike the check for lastObstacle, this is compared with current direction.
    [SerializeField] private float _unstuckAngleMinDuration = 0f;

    private List<Waypoint> _currentPath = new List<Waypoint>();     // This is used to generate the destination path. This is cached for reference after this path is finished.
    private List<Vector2> _destinations = new List<Vector2>();
    private int _destinationIndex = 0;

    // Variables used for unstucking.
    private float _unstuckPositionTimer;
    private float _unstuckAngleTimer;
    private float _unstuckAngleDurationTimer = 0f;
    //private float _lastObstacleReferenceTimer;      // Sets _lastObstacle to null when this timer hits 0.
    private Vector2 _lastPosition;      // Used to unstuck self.
    //private Vector2? _lastObstacle;      // For cases where agents get stuck together.
    private Transform _lastObstacleTransform;       // Used for comparisons.
    private Vector2? _lastObstaclePoint;
    //private float _unstuckAngleOppositeDirection = 1f;
    //private float? _cachedAngleReference;
    //private Vector2 _lastObstacleRotationReference;     // Used to cache _lastObstacle when rotating away it, since _lastObstacle will be reset when this is active.

    public List<Waypoint> CurrentPath => _currentPath;
    public List<Vector2> Destinations => _destinations;
    public int DestinationIndex => _destinationIndex;

    // For Gizmos only
    public Vector2 CurrentDirection => _currentDirection;
    public float AvoidanceCastLength => _avoidanceCastLength;
    public float AvoidanceRadius => _avoidanceRadius;

    public override void Initialize(float movementSpeed, float rotationSpeed, CharacterAgent ownerAgent)
    {
        base.Initialize(movementSpeed, rotationSpeed, ownerAgent);
        if (_initialPath && _initialPath.WaypointList.Any())
        {
            _currentPath = _initialPath.WaypointList;
            GenerateDestinations();
            _unstuckPositionTimer = _unstuckTimerLength;
            _unstuckAngleTimer = _unstuckTimerLength;
        }
    }

    public void Initialize(float movementSpeed, float rotationSpeed, CharacterAgent ownerAgent, WaypointPath newWaypointPath)
    {
        base.Initialize(movementSpeed, rotationSpeed, ownerAgent);
        if (newWaypointPath && newWaypointPath.WaypointList.Any())
        {
            _currentPath = newWaypointPath.WaypointList;
            GenerateDestinations();
            _unstuckPositionTimer = _unstuckTimerLength;
            _unstuckAngleTimer = _unstuckTimerLength;
        }
    }

    // Returns destination point. By default returns the current direction (for weapon rotation purposes).
    public override Vector2 MoveAgent(Transform agentTransform, Rigidbody2D agentRigidbody, float speed, Vector2? customDirection = null)
    {
        if (!_destinations.Any() || _destinationIndex >= _destinations.Count) return _currentDirection;
        // Get target direction.
        // Apply obstacle avoidance to the target direction.
        // Apply steering to agent.
        Vector2 directionToTarget;
        if (customDirection.HasValue)
        {
            directionToTarget = customDirection.Value;
        }
        else
        {
            directionToTarget = _destinations[_destinationIndex] - (Vector2)agentTransform.position;
        }

        // Unstuck logic.
        if (_enableUnstuck)
        {
            if (_enableUnstuckPosition)
            {
                float distanceFromLastPosition = (_lastPosition - (Vector2)agentTransform.position).sqrMagnitude;
                // Unstuck position.
                if (distanceFromLastPosition > _unstuckPositionMinDistance * _unstuckPositionMinDistance)
                {
                    _unstuckPositionTimer = _unstuckTimerLength;
                    _lastPosition = agentTransform.position;
                }
                else _unstuckPositionTimer -= Time.fixedDeltaTime;
            }

            if (_lastObstaclePoint.HasValue)// && _lastObstacleTransform.gameObject.activeInHierarchy)
            {
                float angleToLastObstacle = Vector2.Angle(_currentDirection, _lastObstaclePoint.Value - (Vector2)agentTransform.position);
                //Debug.Log($"CHECKING FOR: {_ownerAgent.transform.name}, ANGLE: {angleToLastObstacle}");
                if (_unstuckAngleTimer > 0f)
                {
                    _unstuckAngleTimer -= Time.fixedDeltaTime;
                }
                //else if (Time.time >= _unstuckAngleDurationTimer && angleToLastObstacle >= _unstuckAngleRotationDifference)
                else if (angleToLastObstacle >= _unstuckAngleRotationDifference)
                        {
                    RemoveObstacleAngle();
                    //Debug.Log($"UNSTUCKANGLES LOGIC! last obs point: {_lastObstaclePoint}, angle: {angleToLastObstacle}, agent name: {_ownerAgent.transform.name}");
                }
            }
        }

        // Move if distance is greater than threshold, else get new waypoint.
        if (directionToTarget.sqrMagnitude > _distanceThreshold * _distanceThreshold)
        {
            // Apply steering and obstacle avoidance to desired direction.
            _currentDirection = GetNextDirection(agentTransform, directionToTarget);
            // Finalize destination for moving.
            Vector2 destination = (Vector2)agentTransform.position + (_currentDirection.normalized * Time.fixedDeltaTime * _movementSpeed);
            // Apply steering and obstacle avoidance to destination.
            StartMovement(agentRigidbody, destination);
            return destination;
        }
        else if (_destinationIndex < _destinations.Count - 1)
        {
            _destinationIndex++;
        }
        //return agentTransform.forward;
        return _currentDirection;
    }

    // Maybe remove the override if I don't need it.
    protected override void StartMovement(Rigidbody2D agentRigidbody, Vector2 destination)
    {
        // Actual logic for moving the agent goes here.
        agentRigidbody.MovePosition(destination);
    }

    private void GenerateDestinations()
    {
        if (!_currentPath.Any()) return;
        _destinations = new List<Vector2>(_currentPath.Count);
        for (int i = 0; i < _currentPath.Count; i++)
        {
            // Get random destination.
            Vector2 newDestination = _currentPath[i].GetRandomArea();

            // Apply bias based on alignment of previous waypoint and current one.
            if (i != 0)
            {
                Waypoint previousWaypoint = _currentPath[i - 1];

                // Get vector to scale old destination with new waypoint's size
                float xBiasScale = _currentPath[i].WaypointSize.x / previousWaypoint.WaypointSize.x;
                float yBiasScale = _currentPath[i].WaypointSize.y / previousWaypoint.WaypointSize.y;

                // Set bias point to local space then apply scaling.
                Vector2 biasPoint = _destinations[i-1] - (Vector2)previousWaypoint.transform.position;
                biasPoint *= new Vector2(xBiasScale, yBiasScale);
                biasPoint += (Vector2)_currentPath[i].transform.position;

                // apply bias to new destination
                newDestination = Vector2.Lerp(newDestination, biasPoint, _laneAlignmentBias);
            }
            _destinations.Add(newDestination);
        }
    }

    // Mainly used by StateMachine to force NPCMover to go to the next waypoint.
    public void ForceIncrementDestinationIndex()
    {
        if (_destinationIndex < _destinations.Count - 1)
        {
            _destinationIndex++;
        }
    }

    // NEW METHOD NAME: RotateWithObstacleAvoidance
    private Vector2 GetNextDirection(Transform agentTransform, Vector2 desiredDirection)
    {
        if (_unstuckPositionTimer <= 0f)
        {
            return RotateDirectionUsingSpeed(_currentDirection, _currentDirection, true);
        }
        else if (_unstuckAngleTimer <= 0f)
        {
            //return RotateDirectionUsingSpeed(_currentDirection, _lastObstacle.Value, true);
            float angleToLastObstacle = Vector2.Angle(_currentDirection, _lastObstaclePoint.Value - (Vector2)agentTransform.position);
            //if (angleToLastObstacle - _cachedAngleReference < _unstuckAngleMaxRotation)
            //{
            Vector2 obstaclePointOffset = ((Vector2)_lastObstacleTransform.position - _lastObstaclePoint.Value);
            //Debug.LogWarning($"ROTATING ANGLES FOR: {_ownerAgent.transform.name}, OBS POINT: {_lastObstaclePoint.Value + obstaclePointOffset}");
            Debug.LogWarning($"ROTATING ANGLES FOR: {_ownerAgent.transform.name}, OBS POINT: {(Vector2)_lastObstacleTransform.position - (Vector2)_ownerAgent.transform.position}");
            //return RotateDirectionUsingSpeed(_currentDirection, (_lastObstaclePoint.Value + obstaclePointOffset) - (Vector2)_ownerAgent.transform.position, true);
            return RotateDirectionUsingSpeed(_currentDirection, (Vector2)_lastObstacleTransform.position - (Vector2)_ownerAgent.transform.position, true);
            //return RotateDirectionUsingSpeed(_currentDirection, _lastObstaclePoint.Value - (Vector2)agentTransform.position, true);
            //}
            //else _unstuckAngleTimer = _unstuckTimerLength;
        }
        //Vector2 directionToTarget = Destination.position - transform.position;
        //Vector2 desiredDirection = desiredDirection;

        // Raycast towards current direction.
        List<RaycastHit2D> obstaclesInPath = Physics2D.CircleCastAll(agentTransform.position, _avoidanceRadius, _currentDirection, _avoidanceCastLength, _steeringLayerMask).ToList();
        bool validObstaclesA = CheckValidObstacles(obstaclesInPath, agentTransform);

        //if (validObstaclesA.Count == 0)
        if (!validObstaclesA)
        {
            // No obstacles in current path.
            Vector2 stepDirection = RotateDirectionUsingSpeed(_currentDirection, desiredDirection);
            // Try to steer towards destination. Steer when no new obstacles were registered, otherwise continue forwards.
            List<RaycastHit2D> obstacleOnRotation = Physics2D.CircleCastAll(agentTransform.position, _avoidanceRadius + _avoidanceRadiusPadding, stepDirection, _avoidanceCastLength, _steeringLayerMask).ToList();
            bool validObstaclesB = CheckValidObstacles(obstacleOnRotation, agentTransform);
            //Debug.LogWarning($"valid obstacle b: {validObstaclesB}, name: {_ownerAgent.transform.name}");

            if (validObstaclesB)
            {
                //Debug.Log($"There are obstacles in new path. Continuing forwards. {agentTransform.name}");
                float newCachedAngle = Vector2.Angle(_currentDirection, obstacleOnRotation[0].point - (Vector2)agentTransform.position);
                //Debug.Log($"cached angle: {newCachedAngle}, condition: {newCachedAngle <= _unstuckObstacleMinAngleThreshold}, threshold: {_unstuckObstacleMinAngleThreshold}, gameobject: {_ownerAgent.transform.name}");
                if (newCachedAngle <= _unstuckObstacleMinAngleThreshold) GetNewObstacleAngle(obstacleOnRotation[0], newCachedAngle);
                return _currentDirection;
                //}
                //if (!_cachedAngleReference.HasValue) _cachedAngleReference = Vector2.Angle(_currentDirection, obstacleOnRotation[0].point - (Vector2)agentTransform.position);
            }
            else
            {
                //Debug.Log($"No obstacles in new path. Rotating towards. {agentTransform.name}");
                RemoveObstacleAngle();
                return RotateDirectionUsingSpeed(_currentDirection, desiredDirection);
            }
        }
        else
        {
            // There's an obstacle in the current direction, so rotate away from the obstacles' center point.
            Vector2 centerPoint = Vector2.zero;
            foreach (RaycastHit2D obstacle in obstaclesInPath)
            {
                centerPoint += obstacle.point;
            }
            // Calculate position to steer away from.
            centerPoint /= obstaclesInPath.Count;
            Vector3 directionToObstacle = centerPoint - (Vector2)agentTransform.position;
            //Debug.Log($"There are obstacles in the current path. Rotating away. {agentTransform.name}");
            //RememberLastObstacle(obstaclesInPath[0].point);
            /*_lastObstacleTransform = null;
            _lastObstaclePoint = null;
            _cachedAngleReference = null;*/
            RemoveObstacleAngle();
            return RotateDirectionUsingSpeed(_currentDirection, directionToObstacle, true);
        }
    }

    private Vector2 RotateDirectionUsingSpeed(Vector2 directionFrom, Vector2 directionTowards, bool avoidDirection = false)
    {
        // Rotation angle is also used to clamp.
        float angleDifference = Vector2.SignedAngle(directionFrom.normalized, directionTowards.normalized);
        float targetAngle = 0f;
        if (avoidDirection) targetAngle = Mathf.Sign(angleDifference) * 180f;
        // Mathf.MoveTowardsAngle made me cry. I'm never using it here ever. EVER.
        float stepAngle = Mathf.MoveTowards(angleDifference, targetAngle, _rotationSpeed * Time.fixedDeltaTime);
        return Quaternion.AngleAxis(angleDifference - stepAngle, Vector3.forward) * directionFrom;
    }

    private void GetNewObstacleAngle(RaycastHit2D obstaclePoint, float angleReference)
    {
        if (!_lastObstacleTransform || _lastObstacleTransform != obstaclePoint.transform)
        {
            _unstuckAngleDurationTimer = Time.time + _unstuckAngleMinDuration;
            //Debug.LogWarning($"OBSTACLEPOINT TRANSFORM: {obstaclePoint.transform}, LASTOBSTACLE TRANSFORM: {_lastObstacleTransform}");
            _lastObstacleTransform = obstaclePoint.transform;
            //_lastObstaclePoint = obstaclePoint.point;
            //_cachedAngleReference = angleReference;
            _unstuckAngleTimer = _unstuckTimerLength;
        }
        _lastObstaclePoint = obstaclePoint.point;
    }

    private void RemoveObstacleAngle()
    {
        _lastObstacleTransform = null;
        _lastObstaclePoint = null;
        //_cachedAngleReference = null;
        _unstuckAngleTimer = _unstuckTimerLength;
    }

    private bool CheckValidObstacles(List<RaycastHit2D> potentialObstacles, Transform agentTransform)
    {
        foreach (RaycastHit2D hit in potentialObstacles)
        {
            if (hit.transform == agentTransform) continue;
            if (hit.transform.TryGetComponent(out CharacterAgent dependencyAgent)
                && !_ownerAgent.DependencyParentAgents.Exists(x => x == dependencyAgent))
            {
                return true;
            }
        }
        return false;
    }

    private void RemoveInvalidObstacles(List<RaycastHit2D> potentialObstacles, Transform agentTransform)
    {
        foreach (RaycastHit2D hit in potentialObstacles)
        {
            if (hit.transform == agentTransform) potentialObstacles.Remove(hit);
            if (hit.transform.TryGetComponent(out CharacterAgent dependencyAgent)
                && _ownerAgent.DependencyParentAgents.Exists(x => x == dependencyAgent))
            {
                potentialObstacles.Remove(hit);
            }
        }
    }
}
