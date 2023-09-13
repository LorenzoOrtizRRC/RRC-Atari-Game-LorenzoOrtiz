using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WaypointMovement : MovementState
{
    [SerializeField] private List<Waypoint> _waypointDestinations = new List<Waypoint>();
    [SerializeField] private float _distanceThreshold = 0.5f;
    [SerializeField, Range(0f, 1f)] private float _laneAlignmentBias = 0f;  // aligns agent to stay on a side of the lane more.
    private int _currentWaypoint = 0;
    private Vector2 _currentDestination = Vector2.zero;

    public void Initialize()
    {
        _currentDestination = GetNewDestination();
    }

    public override void MoveAgent(Transform self, Rigidbody2D rb, float speed)
    {
        Vector2 distance = _currentDestination - (Vector2)self.position;
        // move self if distance is greater than threshold, else get new waypoint
        if (distance.magnitude >= _distanceThreshold)
        {
            rb.MovePosition((Vector2)self.position + (distance.normalized * Time.fixedDeltaTime * speed));
        }
        else
        {
            if (_currentWaypoint == _waypointDestinations.Count - 1) return;
            _currentWaypoint++;
            _currentDestination = GetNewDestination();
        }
    }

    // Works with 2D colliders of any size (box, circle, rectangle). Use box/rectangle and circle colliders.
    private Vector2 GetNewDestination()
    {
        Debug.Log("setting new destination");
        Vector2 newDestination = _waypointDestinations[_currentWaypoint].GetRandomArea();

        if (_currentWaypoint != 0)
        {
            
            // apply bias based on alignment of previous waypoint and current one
            Waypoint oldWaypoint = _waypointDestinations[_currentWaypoint - 1];
            //Vector2 oldDestinationBiasPoint = (_currentDestination - (Vector2)_waypointDestinations[_currentWaypoint - 1].transform.position)
            //    + (Vector2)_waypointDestinations[_currentWaypoint].transform.position;

            // used as target for lerping to implement bias.
            /*
            Vector2 oldExtents = oldWaypoint.WaypointSize / 2f;
            float xBiasFraction = (oldDestinationBiasPoint.x + oldExtents.x) / oldWaypoint.WaypointSize.x + oldExtents.x;
            float yBiasFraction = (oldDestinationBiasPoint.y + oldExtents.y) / oldWaypoint.WaypointSize.y + oldExtents.y;

            // make bias point in new waypoint's area
            Vector2 newExtents = _waypointDestinations[_currentWaypoint].WaypointSize / 2f;    // extents
            float xBias = Mathf.Lerp(-newExtents.x, newExtents.x, xBiasFraction);
            float yBias = Mathf.Lerp(-newExtents.y, newExtents.y, yBiasFraction);
            Vector2 biasPoint = new Vector2(xBias, yBias) + (Vector2)_waypointDestinations[_currentWaypoint].transform.position;
            */
            // Get vector to scale old destination with new waypoint's size
            float xBiasScale = _waypointDestinations[_currentWaypoint].WaypointSize.x / oldWaypoint.WaypointSize.x;
            float yBiasScale = _waypointDestinations[_currentWaypoint].WaypointSize.y / oldWaypoint.WaypointSize.y;
            Debug.LogWarning($"BEFORE INVERSE: {_currentDestination}");
            //Vector2 biasPoint = oldWaypoint.transform.InverseTransformPoint(_currentDestination);
            //Vector2 offset = _waypointDestinations[_currentWaypoint].transform.position - oldWaypoint.transform.position;
            //Debug.LogWarning($"INVERSE using ({oldWaypoint.transform.name}): {biasPoint}");
            Vector2 biasPoint = _currentDestination - (Vector2)oldWaypoint.transform.position;
            biasPoint *= new Vector2(xBiasScale, yBiasScale);
            //biasPoint *= new Vector2(xBiasScale, yBiasScale);
            //biasPoint += offset;
            biasPoint += (Vector2)_waypointDestinations[_currentWaypoint].transform.position;
            //biasPoint += (Vector2)_waypointDestinations[_currentWaypoint].transform.position;
            Debug.LogWarning($"INVERSE + WORLD: {biasPoint}");

            // apply bias to new destination
            newDestination = Vector2.Lerp(newDestination, biasPoint, _laneAlignmentBias);
        }
        return newDestination;
    }
}
