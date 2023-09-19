using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WaypointMovement : MovementState
{
    [SerializeField] private List<Waypoint> _waypointDestinations = new List<Waypoint>();
    [SerializeField] private float _distanceThreshold = 0.3f;
    [SerializeField, Range(0f, 1f)] private float _laneAlignmentBias = 0f;  // aligns agent to stay on a side of the lane more.
    private int _waypointIndex = 0;
    private Vector2 _currentDestination = Vector2.zero;

    public void SetWaypoints(List<Waypoint> initialWaypoints) => _waypointDestinations = initialWaypoints;

    public override void Initialize()
    {
        _currentDestination = GetNewDestination();
    }

    // returns destination point. by default returns forward (for weapon rotation purposes).
    public override Vector2 MoveAgent(Transform self, Rigidbody2D rb, float speed, Vector3? customDestination = null)
    {
        Vector2 distance = _currentDestination - (Vector2)self.position;
        // move self if distance is greater than threshold, else get new waypoint
        if (distance.magnitude >= _distanceThreshold)
        {
            Vector2 destination = (Vector2)self.position + (distance.normalized * Time.deltaTime * speed);
            rb.MovePosition(destination);
            return destination;
        }
        else if (_waypointIndex != _waypointDestinations.Count - 1)
        {
            _waypointIndex++;
            _currentDestination = GetNewDestination();
        }
        return self.forward;
    }

    // Works with 2D colliders of any size (box, circle, rectangle). Use box/rectangle and circle colliders.
    private Vector2 GetNewDestination()
    {
        Vector2 newDestination = _waypointDestinations[_waypointIndex].GetRandomArea();

        if (_waypointIndex != 0)
        {
            // apply bias based on alignment of previous waypoint and current one
            Waypoint oldWaypoint = _waypointDestinations[_waypointIndex - 1];

            // Get vector to scale old destination with new waypoint's size
            float xBiasScale = _waypointDestinations[_waypointIndex].WaypointSize.x / oldWaypoint.WaypointSize.x;
            float yBiasScale = _waypointDestinations[_waypointIndex].WaypointSize.y / oldWaypoint.WaypointSize.y;
            // set bias point to local space then apply scales
            Vector2 biasPoint = _currentDestination - (Vector2)oldWaypoint.transform.position;
            biasPoint *= new Vector2(xBiasScale, yBiasScale);
            biasPoint += (Vector2)_waypointDestinations[_waypointIndex].transform.position;

            // apply bias to new destination
            newDestination = Vector2.Lerp(newDestination, biasPoint, _laneAlignmentBias);
        }
        return newDestination;
    }
}
