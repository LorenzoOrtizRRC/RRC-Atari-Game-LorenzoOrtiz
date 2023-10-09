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
    [SerializeField] private float _unstuckTimerLength = 0.5f;       // When position is within _unstuckMinDistance of _lastPosition, timer will start. Unstucking starts when timer hits 0.
    [SerializeField] private float _unstuckPositionMinDistance = 0.2f;      // When distance from _lastPosition is greater than this, _lastPosition will be reset.
    [SerializeField] private float _unstuckObstacleMinAngle = 2f;      // Mostly used for cases where agents get stuck together.
    [SerializeField] private float _unstuckAngleMaxRotation = 45f;      // Mostly used for cases where agents get stuck together. Unlike the check for lastObstacle, this is compared with current direction.

    private List<Waypoint> _currentPath = new List<Waypoint>();     // This is used to generate the destination path. This is cached for reference after this path is finished.
    private List<Vector2> _destinations = new List<Vector2>();
    private int _destinationIndex = 0;

    // Variables used for unstucking.
    private float _unstuckPositionTimer;
    private float _unstuckAngleTimer;
    private float _lastObstacleReferenceTimer;      // Sets _lastObstacle to null when this timer hits 0.
    private Vector2 _lastPosition;      // Used to unstuck self.
    private Vector2? _lastObstacle;      // For cases where agents get stuck together.
    private Vector2? lastObstacle2;
    private float? _cachedAngleReference;
    //private Vector2 _lastObstacleRotationReference;     // Used to cache _lastObstacle when rotating away it, since _lastObstacle will be reset when this is active.

    public List<Waypoint> CurrentPath => _currentPath;
    public List<Vector2> Destinations => _destinations;
    public int DestinationIndex => _destinationIndex;

    // For Gizmos only
    public Vector2 CurrentDirection => _currentDirection;
    public float AvoidanceCastLength => _avoidanceCastLength;
    public float AvoidanceRadius => _avoidanceRadius;

    //public void SetWaypoints(WaypointPath initialPath) => _waypointPath = initialPath;

    public void Initialize()
    {
        if (_initialPath && _initialPath.WaypointList.Any())
        {
            _currentPath = _initialPath.WaypointList;
            GenerateDestinations();
            _unstuckAngleTimer = _unstuckTimerLength;
        }
    }

    public void Initialize(WaypointPath newWaypointPath)
    {
        if (newWaypointPath && newWaypointPath.WaypointList.Any())
        {
            _currentPath = newWaypointPath.WaypointList;
            GenerateDestinations();
            _unstuckAngleTimer = _unstuckTimerLength;
        }
    }

    // Returns destination point. By default returns the transform local forward (for weapon rotation purposes).
    public override Vector2 MoveAgent(Transform agentTransform, Rigidbody2D agentRigidbody, float speed, Vector2? customDirection = null)
    {
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
            float distanceFromLastPosition = (_lastPosition - (Vector2)agentTransform.position).sqrMagnitude;
            //float angleFromLastObstacle = Vector2.Angle(agentTransform.position, _lastObstacle.Value);
            // Unstuck position.
            if (distanceFromLastPosition > _unstuckPositionMinDistance * _unstuckPositionMinDistance)
            {
                _unstuckPositionTimer = _unstuckTimerLength;
                _lastPosition = agentTransform.position;
            }
            else _unstuckPositionTimer -= Time.fixedDeltaTime;/*
            // Manage last obstacle reference for use with unstucking angles.
            if (_lastObstacle.HasValue && _lastObstacleReferenceTimer <= 0f) _lastObstacle = null;
            else _lastObstacleReferenceTimer -= Time.fixedDeltaTime;
            // Unstuck angles.
            if (_lastObstacle.HasValue)// && angleFromLastObstacle < _unstuckObstacleMinAngle)
            {
                float angleFromLastObstacle = Vector2.Angle(agentTransform.position, _lastObstacle.Value);
                if (angleFromLastObstacle > _unstuckObstacleMinAngle)
                {
                    _unstuckAngleTimer = _unstuckTimerLength;
                }
                else _unstuckAngleTimer -= Time.fixedDeltaTime;
            }
            else _unstuckAngleTimer = _unstuckTimerLength;*/
            if (lastObstacle2.HasValue)
            {
                float angleToLastObstacle = Vector2.Angle(_currentDirection, lastObstacle2.Value - (Vector2)agentTransform.position);
                Debug.Log($"angle to last: {angleToLastObstacle}, cached angle: {_cachedAngleReference}, condition: {angleToLastObstacle < _cachedAngleReference + _unstuckObstacleMinAngle && angleToLastObstacle > _cachedAngleReference - _unstuckObstacleMinAngle}");
                Debug.Log($"max range: {_cachedAngleReference + _unstuckObstacleMinAngle}, min range: {_cachedAngleReference - _unstuckObstacleMinAngle}");
                if (angleToLastObstacle < _cachedAngleReference + _unstuckObstacleMinAngle
                    && angleToLastObstacle > _cachedAngleReference - _unstuckObstacleMinAngle)
                {
                    _unstuckAngleTimer -= Time.fixedDeltaTime;
                }
                else if (angleToLastObstacle > _cachedAngleReference + _unstuckAngleMaxRotation
                    || angleToLastObstacle > _cachedAngleReference - _unstuckAngleMaxRotation)
                {
                    _unstuckAngleTimer = _unstuckTimerLength;
                    lastObstacle2 = null;
                    _cachedAngleReference = null;
                    //if (lastObstacle2.HasValue) Debug.Log(Vector2.Angle(lastObstacle2.Value, agentTransform.position));
                }
            }
        }
        // Move stuff here.
        //_currentDirection = GetNextDirection();
        //_rb.MovePosition((Vector2)transform.position + _currentDirection * Time.deltaTime * _speed);

        // Move if distance is greater than threshold, else get new waypoint.
        if (directionToTarget.sqrMagnitude > _distanceThreshold * _distanceThreshold)
        {
            // Apply steering and obstacle avoidance to desired direction.
            _currentDirection = GetNextDirection(agentTransform, directionToTarget);
            // Finalize destination for moving.
            Vector2 destination = (Vector2)agentTransform.position + (_currentDirection.normalized * Time.fixedDeltaTime * _movementSpeed);
            // Apply steering and obstacle avoidance to destination.
            //destination = GetNextDirection(agentTransform, destination);
            //agentRigidbody.MovePosition(destination);
            StartMovement(agentRigidbody, destination);
            return destination;
        }
        else if (_destinationIndex < _destinations.Count - 1)
        {
            _destinationIndex++;
        }
        return agentTransform.forward;
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
            float angleToLastObstacle = Vector2.Angle(_currentDirection, lastObstacle2.Value - (Vector2)agentTransform.position);
            //if (angleToLastObstacle - _cachedAngleReference < _unstuckAngleMaxRotation)
            //{
                return RotateDirectionUsingSpeed(_currentDirection, lastObstacle2.Value, true);
            //}
            //else _unstuckAngleTimer = _unstuckTimerLength;
        }
        //Vector2 directionToTarget = Destination.position - transform.position;
        //Vector2 desiredDirection = desiredDirection;

        // Raycast towards current direction.
        Vector2 stepDirection = RotateDirectionUsingSpeed(_currentDirection, desiredDirection);
        List<RaycastHit2D> obstaclesInPath = Physics2D.CircleCastAll(agentTransform.position, _avoidanceRadius, _currentDirection, _avoidanceCastLength, _steeringLayerMask).ToList();
        // Remove self from raycast.
        RaycastHit2D selfHit = obstaclesInPath.Find(x => x.transform == agentTransform);
        //List<RaycastHit2D> selfHits = obstaclesInPath.FindAll(x => x.transform == agentTransform);
        if (selfHit) obstaclesInPath.Remove(selfHit);

        if (obstaclesInPath.Count == 0)
        {
            // No obstacles in current path.
            // Try to steer towards destination. Steer when no new obstacles were registered, otherwise continue forwards.
            List<RaycastHit2D> obstacleOnRotation = Physics2D.CircleCastAll(agentTransform.position, _avoidanceRadius + _avoidanceRadiusPadding, stepDirection, _avoidanceCastLength, _steeringLayerMask).ToList();
            // Remove self from raycast.
            RaycastHit2D selfHitB = obstacleOnRotation.Find(x => x.transform == agentTransform);
            if (selfHitB) obstacleOnRotation.Remove(selfHitB);

            if (obstacleOnRotation.Any())
            {
                //Debug.Log($"There are obstacles in new path. Continuing forwards. {agentTransform.name}");
                RememberLastObstacle(obstacleOnRotation[0].point);
                lastObstacle2 = obstacleOnRotation[0].point;

                if (!_cachedAngleReference.HasValue) _cachedAngleReference = Vector2.Angle(_currentDirection, obstacleOnRotation[0].point - (Vector2)agentTransform.position);
                return _currentDirection;
            }
            else
            {
                //Debug.Log($"No obstacles in new path. Rotating towards. {agentTransform.name}");
                lastObstacle2 = null;
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
            RememberLastObstacle(obstaclesInPath[0].point);
            lastObstacle2 = null;
            return RotateDirectionUsingSpeed(_currentDirection, directionToObstacle, true);
        }
    }

    private Vector2 RotateDirectionUsingSpeed(Vector2 directionFrom, Vector2 directionTowards, bool avoidDirection = false)
    {
        float rotationAngle = Vector2.SignedAngle(directionFrom.normalized, directionTowards.normalized);
        if (rotationAngle == 0f && avoidDirection)
        {
            rotationAngle = 45f;
        }
        float stepAngle = Mathf.MoveTowardsAngle(0f, rotationAngle, _rotationSpeed * Time.fixedDeltaTime);
        if (avoidDirection) stepAngle = -stepAngle;
        return Quaternion.AngleAxis(stepAngle, Vector3.forward) * directionFrom;
    }

    private void RememberLastObstacle(Vector2 lastObstaclePosition)
    {
        _lastObstacle = lastObstaclePosition;
        _lastObstacleReferenceTimer = _unstuckTimerLength;
    }
}
