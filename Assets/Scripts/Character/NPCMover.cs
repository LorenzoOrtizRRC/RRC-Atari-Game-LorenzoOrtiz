using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCMover : CharacterMover
{
    [Header("Movement Variables")]
    [SerializeField] private float _distanceThreshold = 0.3f;
    [SerializeField, Range(0f, 1f)] private float _laneAlignmentBias = 0f;  // Aligns agent to stay on a side of the lane more.
    private WaypointPath _waypointPath;
    private int _waypointIndex = 0;
    private Vector2 _currentDestination = Vector2.zero;

    public Waypoint CurrentWaypoint => _waypointPath.WaypointList[_waypointIndex];

    public void SetWaypoints(WaypointPath initialPath) => _waypointPath = initialPath;

    public virtual void Initialize()
    {
        _currentDestination = GetNewDestination();
    }

    // Returns destination point. By default returns forward (for weapon rotation purposes).
    public virtual Vector2 MoveAgent(Transform agentTransform, Rigidbody2D agentRigidbody, float speed, Vector3? customDirection = null)
    {
        Vector2 directionToDestination = _currentDestination - (Vector2)agentTransform.position;
        // Move if distance is greater than threshold, else get new waypoint
        if (directionToDestination.magnitude >= _distanceThreshold)
        {
            Vector2 destination = (Vector2)agentTransform.position + (directionToDestination.normalized * Time.fixedDeltaTime * speed);
            agentRigidbody.MovePosition(destination);
            return destination;
        }
        else if (_waypointIndex < _waypointPath.WaypointList.Count - 1)
        {
            _waypointIndex++;
            _currentDestination = GetNewDestination();
        }
        return agentTransform.forward;
    }

    public void ForceIncrementWaypointIndex()
    {
        if (_waypointIndex < _waypointPath.WaypointList.Count - 1)
        {
            _waypointIndex++;
            _currentDestination = GetNewDestination();
        }
    }

    // Works with 2D box colliders only.
    private Vector2 GetNewDestination()
    {
        Vector2 newDestination = _waypointPath.WaypointList[_waypointIndex].GetRandomArea();

        if (_waypointIndex != 0)
        {
            // apply bias based on alignment of previous waypoint and current one
            Waypoint oldWaypoint = _waypointPath.WaypointList[_waypointIndex - 1];

            // Get vector to scale old destination with new waypoint's size
            float xBiasScale = _waypointPath.WaypointList[_waypointIndex].WaypointSize.x / oldWaypoint.WaypointSize.x;
            float yBiasScale = _waypointPath.WaypointList[_waypointIndex].WaypointSize.y / oldWaypoint.WaypointSize.y;
            // set bias point to local space then apply scales
            Vector2 biasPoint = _currentDestination - (Vector2)oldWaypoint.transform.position;
            biasPoint *= new Vector2(xBiasScale, yBiasScale);
            biasPoint += (Vector2)_waypointPath.WaypointList[_waypointIndex].transform.position;

            // apply bias to new destination
            newDestination = Vector2.Lerp(newDestination, biasPoint, _laneAlignmentBias);
        }
        return newDestination;
    }
}
