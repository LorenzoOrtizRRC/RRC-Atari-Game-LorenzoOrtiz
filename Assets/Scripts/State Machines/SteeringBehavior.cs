using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class SteeringBehavior : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D _rb;
    [Header("Steering Variables")]
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _targetDistanceThreshold = 0.5f;
    [SerializeField, Min(1)] private int _steeringQuadrantsCount = 1;
    [SerializeField] private LayerMask _steeringLayerMask;

    public Transform Destination;
    public float SteeringSpeedTest = 10f;
    public float AvoidanceCastDistance = 2f;
    public float UnstuckTimerStartDelay = 1f;
    //public float minAvoidanceDistanceThreshold = 3f;
    //public float maxAvoidanceDistanceThreshold = 0.5f;

    //private List<Transform> _steeringObstacles = new List<Transform>();
    private Vector2 _currentDirection = Vector2.up;
    //private Vector2 _obstaclesCenter = Vector2.zero;

    //private Vector2[] directionVectors = new Vector2[0];
    //private float[] directionWeights = new float[0];

    private Vector2 _oldDirectionToObstacle;
    private float unstuckTimer;
    public int _multiObstacleRayInterval = 5;
    private int _multiObstacleRayCounter = 0;

    private void OnEnable()
    {
        _currentDirection = transform.up;
        unstuckTimer = UnstuckTimerStartDelay;
    }
    private void FixedUpdate()
    {
        
        Vector2 direction = Quaternion.AngleAxis(SteeringSpeedTest * Time.fixedDeltaTime, Vector3.forward) * _currentDirection;
        //_currentDirection = direction;

        //Vector2 preferredDirection = GetNextDirection();
        //_currentDirection = RotateDirectionUsingSpeed(_currentDirection, preferredDirection);
        float distanceFromTarget = (Destination.position - transform.position).magnitude;
        if (Destination && distanceFromTarget > _targetDistanceThreshold)
        {
            _currentDirection = GetNextDirection();
            _rb.MovePosition((Vector2)transform.position + _currentDirection * Time.fixedDeltaTime * _speed);
        }

        // Rotate towards direction
        //float rotationAngle = Vector2.SignedAngle(_currentDirection.normalized, preferredDirection.normalized);
        //float stepAngle = Mathf.MoveTowardsAngle(0f, rotationAngle, SteeringSpeedTest * Time.fixedDeltaTime);
        //_currentDirection = Quaternion.AngleAxis(stepAngle, Vector3.forward) * _currentDirection;
        //_rb.MovePosition((Vector2)transform.position + _currentDirection * Time.fixedDeltaTime * _speed);    
    }

    private Vector2 GetNextDirection()
    {
        Vector2 directionToTarget = Destination.position - transform.position;
        // Current dirrection initally points towards destination.
        // Raycast towards current direction.
        Vector2 stepDirection = RotateDirectionUsingSpeed(_currentDirection, directionToTarget);
        float selfRadius = GetComponent<CircleCollider2D>().radius;
        List<RaycastHit2D> obstaclesInPath = Physics2D.CircleCastAll(transform.position, selfRadius, _currentDirection, AvoidanceCastDistance, _steeringLayerMask).ToList();

        RaycastHit2D selfHit = obstaclesInPath.Find(x => x.transform == transform);
        if (selfHit) obstaclesInPath.Remove(selfHit);

        if (obstaclesInPath.Count == 0)
        {
            print("NO OBSTACLES IN PATH");
            // Try to steer towards destination. Steer when no new obstacles were registered, otherwise continue forwards.
            List<RaycastHit2D> obstacleOnRotation = Physics2D.CircleCastAll(transform.position, selfRadius, stepDirection, AvoidanceCastDistance, _steeringLayerMask).ToList();
            RaycastHit2D selfHitB = obstacleOnRotation.Find(x => x.transform == transform);
            if (selfHitB) obstaclesInPath.Remove(selfHitB);
            //print($"new obstacle exists: {obstacleOnRotation}, is self: {obstacleOnRotation.transform != transform}, bool: {obstacleOnRotation && obstacleOnRotation.transform != transform}");
            if (obstacleOnRotation.Any())// && obstacleOnRotation.transform != transform)
            {
                print("OBSTACLES IN NEW PATH");
                return _currentDirection;
            }
            else
            {
                return RotateDirectionUsingSpeed(_currentDirection, directionToTarget);
            }
        }
        else
        {
            // There's an obstacle in the current direction, so rotate.
            string debugString = "";
            Vector2 centerPoint = Vector2.zero;
            foreach (RaycastHit2D obstacle in obstaclesInPath)
            {
                centerPoint += obstacle.point;
                debugString += $" {obstacle.transform.name}";
            }
            Debug.Log(debugString);

            // Calculate position to steer away from.
            centerPoint /= obstaclesInPath.Count;
            Vector3 directionToObstacle = centerPoint - (Vector2)transform.position;
            if (_multiObstacleRayCounter == 0)
            {
                // Counter checks.
                //_oldDirectionToObstacle = (directionToTarget + directionToTarget+ (Vector2)directionToObstacle)/3f;
                _oldDirectionToObstacle = directionToObstacle;
                if (obstaclesInPath.Count > 1)
                {
                    //_oldDirectionToObstacle = directionToObstacle;
                    _multiObstacleRayCounter = _multiObstacleRayInterval;
                    return RotateDirectionUsingSpeed(_currentDirection, _oldDirectionToObstacle, true);
                    //return RotateDirectionUsingSpeed(_currentDirection, directionToTarget, true);
                }
                else
                {
                    //_oldDirectionToObstacle = (_oldDirectionToObstacle + directionToTarget + directionToTarget) / 3f;
                    //return RotateDirectionUsingSpeed(_currentDirection, _oldDirectionToObstacle, false);
                }
            }
            else
            {
                _multiObstacleRayCounter--;
            }
            return RotateDirectionUsingSpeed(_currentDirection, _oldDirectionToObstacle, true);
            //return RotateDirectionUsingSpeed(_currentDirection, directionToTarget, true);
        }
    }

    private Vector2 RotateDirectionUsingSpeed(Vector2 directionFrom, Vector2 directionTowards, bool inverseDirection = false)
    {
        float rotationAngle = Vector2.SignedAngle(directionFrom.normalized, directionTowards.normalized);
        if (rotationAngle == 0f && inverseDirection)
        {
            Debug.LogWarning("ANGLE IS 0");
            rotationAngle = 45f;
        }
        float stepAngle = Mathf.MoveTowardsAngle(0f, rotationAngle, SteeringSpeedTest * Time.fixedDeltaTime);
        if (inverseDirection) stepAngle = -stepAngle;
        return Quaternion.AngleAxis(stepAngle, Vector3.forward) * directionFrom;
    }

    /*
    private Vector2 GetNewDirection()
    {
        if (!Destination) return Vector2.zero;
        Vector2 targetDirection = Destination.position - transform.position;
        // Generate potential directions, and generate weights based on dot product.
        float directionIncrement = 360 / (4 * _steeringQuadrantsCount);
        int directionCount = Mathf.Clamp((4 * _steeringQuadrantsCount), 4, 360);
        directionVectors = new Vector2[directionCount];
        directionWeights = new float[directionCount];

        for (int x = 0; x < directionCount; x++)
        {
            // Assign direction in the form of angles.
            float directionAngle = directionIncrement * x;
            Vector2 directionToEvaluate = Quaternion.AngleAxis(directionAngle, Vector3.forward) * _currentDirection;
            directionVectors[x] = directionToEvaluate;
            // Apply weights based on Vector2.Dot, compare potential directions with direction to target destination.

            // Calculate direction weight.
            //float directionDot = Vector2.Dot(directionVectors[x].normalized, targetDirection.normalized);
            // Normalize dot product as weight from 0 to 1.
            //directionWeights[x] = (directionDot + 1f) / 2f;
            directionWeights[x] = 1f;       // Flat weight to modify.
        }

        // Modify the weights based on steering obstacles.
        if (_steeringObstacles.Count != 0)
        {
            // Get average of all obstacles
            Vector2 obstaclesCenterPosition = Vector2.zero;
            float totalObstacleWeight = 0f;
            float highestWeight = 0f;
            for (int obstacleIndex = 0; obstacleIndex < _steeringObstacles.Count; obstacleIndex++)
            {
                Vector2 obstacleDirection = _steeringObstacles[obstacleIndex].position - transform.position;
                float weightMultiplier = Mathf.Clamp01((obstacleDirection.magnitude - maxAvoidanceDistanceThreshold)
                    / (minAvoidanceDistanceThreshold - maxAvoidanceDistanceThreshold));
                weightMultiplier = 1 - weightMultiplier;
                print("weight multiplier: " + weightMultiplier + ", magnitude: " + obstacleDirection.magnitude);
                Vector2 weightedPosition = (Vector2)_steeringObstacles[obstacleIndex].position * weightMultiplier;
                if (weightMultiplier > highestWeight) highestWeight = weightMultiplier;
                totalObstacleWeight += weightMultiplier;
                obstaclesCenterPosition += weightedPosition;
            }
            obstaclesCenterPosition /= totalObstacleWeight;

            Vector2 avoidanceCenter = obstaclesCenterPosition - (Vector2)transform.position;
            _obstaclesCenter = avoidanceCenter;
            for (int i = 0; i < directionWeights.Length; i++)
            {
                // Modify weights based on obstacles present.
                float weightPenalty = Vector2.Dot(directionVectors[i].normalized, avoidanceCenter.normalized);

                directionWeights[i] = Mathf.Clamp01(directionWeights[i] - (weightPenalty + 0.25f));
            }
        }

        // Modify weights based on destination.
        for (int i = 0; i < directionWeights.Length; i++)
        {
            float directionDot = Vector2.Dot(directionVectors[i].normalized, targetDirection.normalized);
            // Normalize dot product as weight from 0 to 1.
            directionWeights[i] *= (directionDot + 1f) / 2f;
            //directionWeights[i] *= directionDot;
        }

        // Get direction with highest weight.
        int highestWeightIndex = Array.IndexOf(directionWeights, directionWeights.Max());
        Vector2 preferredDirection = directionVectors[highestWeightIndex];

        return preferredDirection;
    }
    */
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        _steeringObstacles.Add(collision.gameObject.transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _steeringObstacles.Remove(collision.gameObject.transform);
    }
    */
    private void MoveAgent(Vector2 normalizedDirection)
    {
        _rb.MovePosition(normalizedDirection * Time.fixedDeltaTime * _speed);
    }

    private void OnDrawGizmos()
    {
        // GIZMOS
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, _currentDirection.normalized * 4f);
        Gizmos.color = Color.yellow;
        //Gizmos.DrawRay(transform.position, (Vector3)_obstaclesCenter);
        Gizmos.color = Color.green;
        float selfRadius = GetComponent<CircleCollider2D>().radius;
        Gizmos.DrawWireSphere((Vector2)transform.position + (_currentDirection.normalized * AvoidanceCastDistance), selfRadius);
        /*
        if (directionVectors.Length == 0) return;
        for (int i = 0; i < directionVectors.Length; i++)
        {
            Gizmos.DrawRay(transform.position, (directionVectors[i] * (directionWeights[i]) * 2f));
            //int angleIncrement = 360 / directionVectors.Length;
            //print($"current angle: {angleIncrement * (i + 1)}, weight: {directionWeights[i]}, direction vector: {directionVectors[i].normalized}");
        }
        */
    }
}
