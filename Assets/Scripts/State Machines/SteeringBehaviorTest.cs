using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class SteeringBehaviorTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D _rb;
    [Header("Steering Variables")]
    public Transform Destination;
    [SerializeField] private LayerMask _steeringLayerMask;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _targetDistanceThreshold = 0.5f;
    [SerializeField] private float _steeringSpeedTest = 180f;
    [SerializeField] private float _avoidanceCastLength = 1f;
    [SerializeField] private float _avoidanceRadius = 1f;
    [SerializeField] private float _unstuckTimerDelay = 0.3f;       // When position is within _unstuckMinDistance of _lastPosition, timer will start. Unstucking starts when timer hits 0.
    [SerializeField] private float _unstuckMinDistance = 0.2f;      // When distance from _lastPosition is greater than this, _lastPosition will be reset.
    [SerializeField] private bool _enableUnstuck = true;            // Must be turned off for static/immovable agents.

    private Vector2 _currentDirection = Vector2.up;
    private float unstuckTimer;
    private Vector2 _lastPosition;      // Used to unstuck self.

    private void OnEnable()
    {
        _currentDirection = transform.up;
        unstuckTimer = _unstuckTimerDelay;
    }
    private void FixedUpdate()
    {
        // Unstucking calculations.
        float distanceFromTarget = (Destination.position - transform.position).magnitude;
        if (Destination && distanceFromTarget > _targetDistanceThreshold)
        {
            if (_enableUnstuck)
            {
                if ((_lastPosition - (Vector2)transform.position).sqrMagnitude >= _unstuckMinDistance * _unstuckMinDistance)
                {
                    unstuckTimer = _unstuckTimerDelay;
                    _lastPosition = transform.position;
                }
                else unstuckTimer -= Time.fixedDeltaTime;
            }
            _currentDirection = GetNextDirection();
            _rb.MovePosition((Vector2)transform.position + _currentDirection * Time.fixedDeltaTime * _speed);
        } 
    }

    private Vector2 GetNextDirection()
    {
        if (unstuckTimer <= 0)
        {
            return RotateDirectionUsingSpeed(_currentDirection, _currentDirection, true);
        }
        Vector2 directionToTarget = Destination.position - transform.position;

        // Raycast towards current direction.
        Vector2 stepDirection = RotateDirectionUsingSpeed(_currentDirection, directionToTarget);
        List<RaycastHit2D> obstaclesInPath = Physics2D.CircleCastAll(transform.position, _avoidanceRadius, _currentDirection, _avoidanceCastLength, _steeringLayerMask).ToList();

        RaycastHit2D selfHit = obstaclesInPath.Find(x => x.transform == transform);
        if (selfHit) obstaclesInPath.Remove(selfHit);

        if (obstaclesInPath.Count == 0)
        {
            // No obstacles in current path.
            // Try to steer towards destination. Steer when no new obstacles were registered, otherwise continue forwards.
            List<RaycastHit2D> obstacleOnRotation = Physics2D.CircleCastAll(transform.position, _avoidanceRadius, stepDirection, _avoidanceCastLength, _steeringLayerMask).ToList();
            // Remove self from raycast.
            RaycastHit2D selfHitB = obstacleOnRotation.Find(x => x.transform == transform);
            if (selfHitB) obstacleOnRotation.Remove(selfHitB);

            if (obstacleOnRotation.Any())
            {
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
            Vector2 centerPoint = Vector2.zero;
            foreach (RaycastHit2D obstacle in obstaclesInPath)
            {
                centerPoint += obstacle.point;
            }
            // Calculate position to steer away from.
            centerPoint /= obstaclesInPath.Count;
            Vector3 directionToObstacle = centerPoint - (Vector2)transform.position;
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
        float stepAngle = Mathf.MoveTowardsAngle(0f, rotationAngle, _steeringSpeedTest * Time.fixedDeltaTime);
        if (avoidDirection) stepAngle = -stepAngle;
        return Quaternion.AngleAxis(stepAngle, Vector3.forward) * directionFrom;
    }

    private void MoveAgent(Vector2 normalizedDirection)
    {
        _rb.MovePosition(normalizedDirection * Time.fixedDeltaTime * _speed);
    }

    private void OnDrawGizmos()
    {
        // GIZMOS
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, _currentDirection.normalized * 4f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position + (_currentDirection.normalized * _avoidanceCastLength), _avoidanceRadius);
    }
}
