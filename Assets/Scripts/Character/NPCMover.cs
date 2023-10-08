using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class NPCMover : CharacterMover
{
    [Header("Movement Variables")]
    [SerializeField] private WaypointPath _initialPath = null;      // Mainly used when NPC is already in the level and not spawned.
    [SerializeField] private float _distanceThreshold = 0.5f;
    [SerializeField, Range(0f, 1f)] private float _laneAlignmentBias = 0f;  // Aligns agent to stay on a side of the lane more.
    //private WaypointPath _waypointPath;
    private List<Waypoint> _currentPath = new List<Waypoint>();     // This is used to generate the destination path. This is cached for reference after this path is finished.
    private List<Vector2> _destinations = new List<Vector2>();
    private int _destinationIndex = 0;
    //private Vector2 _currentDestination = Vector2.zero;

    //public Waypoint CurrentWaypoint => _waypointPath.WaypointList[_destinationIndex];
    public List<Waypoint> CurrentPath => _currentPath;
    public List<Vector2> Destinations => _destinations;
    public int DestinationIndex => _destinationIndex;

    //public void SetWaypoints(WaypointPath initialPath) => _waypointPath = initialPath;

    public void Initialize()
    {
        //_currentDestination = GetNewDestination();
        if (_currentPath.Any()) _currentPath = _initialPath.WaypointList;
        Debug.Log($"INIT 1.1: paths count: {_currentPath.Count}, destinations count: {_destinations.Count}");
        GenerateDestinations();
        Debug.Log($"INIT 1.2: paths count: {_currentPath.Count}, destinations count: {_destinations.Count}");
    }

    public void Initialize(WaypointPath newWaypointPath)
    {
        if (newWaypointPath.WaypointList.Any()) _currentPath = newWaypointPath.WaypointList;
        Debug.Log($"INIT 2.1: paths count: {_currentPath.Count}, destinations count: {_destinations.Count}");
        GenerateDestinations();
        Debug.Log($"INIT 2.2: paths count: {_currentPath.Count}, destinations count: {_destinations.Count}");
    }

    // Returns destination point. By default returns the transform local forward (for weapon rotation purposes).
    public override Vector2 MoveAgent(Transform agentTransform, Rigidbody2D agentRigidbody, float speed, Vector2? customDirection = null)
    {
        Vector2 directionToDestination = _destinations[_destinationIndex] - (Vector2)agentTransform.position;
        // Move if distance is greater than threshold, else get new waypoint
        if (directionToDestination.sqrMagnitude >= _distanceThreshold * _distanceThreshold)
        {
            Vector2 destination = (Vector2)agentTransform.position + (directionToDestination.normalized * Time.fixedDeltaTime * speed);
            agentRigidbody.MovePosition(destination);
            return destination;
        }
        else if (_destinationIndex < _destinations.Count - 1)
        {
            _destinationIndex++;
            //_currentDestination = GetNewDestination();
        }
        return agentTransform.forward;
    }

    public void GenerateDestinations()
    {
        //_destinations.Clear();
        _destinations = new List<Vector2>(_currentPath.Count);
        for (int i = 0; i < _currentPath.Count; i++)
        {
            Debug.Log($"i: {i}");
            // Get random destination.
            Vector2 newDestination = _currentPath[i].GetRandomArea();
            // Get scale multipliers.
            if (i != 0)
            {
                /*
            Waypoint previousWaypoint = _currentPath[i - 1];
            Vector2 localOldDestination = oldDestination - (Vector2)previousWaypoint.transform.position;
            // Formula for bias multiplier (X as example): (-extent + localOldDestination.x + extent) / (extent + extent).
            float biasMultiplierX = (localOldDestination.x + (previousWaypoint.WaypointSize.x / 2f)) / previousWaypoint.WaypointSize.x;
            float biasMultiplierY = (localOldDestination.y + (previousWaypoint.WaypointSize.y / 2f)) / previousWaypoint.WaypointSize.y;
            // Apply multipliers.
            newDestination.x = */

                // apply bias based on alignment of previous waypoint and current one
                Waypoint previousWaypoint = _currentPath[i - 1];

                // Get vector to scale old destination with new waypoint's size
                float xBiasScale = _currentPath[i].WaypointSize.x / previousWaypoint.WaypointSize.x;
                float yBiasScale = _currentPath[i].WaypointSize.y / previousWaypoint.WaypointSize.y;
                // set bias point to local space then apply scales
                Vector2 biasPoint = _destinations[i-1] - (Vector2)previousWaypoint.transform.position;
                biasPoint *= new Vector2(xBiasScale, yBiasScale);
                biasPoint += (Vector2)_currentPath[i].transform.position;

                // apply bias to new destination
                newDestination = Vector2.Lerp(newDestination, biasPoint, _laneAlignmentBias);
            }
            _destinations.Add(newDestination);
        }
    }

    public void ForceIncrementWaypointIndex()
    {
        if (_destinationIndex < _destinations.Count - 1)
        {
            _destinationIndex++;
            //_currentDestination = GetNewDestination();
        }
    }

    /*
    private Vector2 GetDestination(Waypoint waypoint)
    {
        Vector2 newDestination = _waypointPath.WaypointList[_destinationIndex].GetRandomArea();

        if (_destinationIndex != 0)
        {
            // apply bias based on alignment of previous waypoint and current one
            Waypoint oldWaypoint = _waypointPath.WaypointList[_destinationIndex - 1];

            // Get vector to scale old destination with new waypoint's size
            float xBiasScale = _waypointPath.WaypointList[_destinationIndex].WaypointSize.x / oldWaypoint.WaypointSize.x;
            float yBiasScale = _waypointPath.WaypointList[_destinationIndex].WaypointSize.y / oldWaypoint.WaypointSize.y;
            // set bias point to local space then apply scales
            Vector2 biasPoint = _currentDestination - (Vector2)oldWaypoint.transform.position;
            biasPoint *= new Vector2(xBiasScale, yBiasScale);
            biasPoint += (Vector2)_waypointPath.WaypointList[_destinationIndex].transform.position;

            // apply bias to new destination
            newDestination = Vector2.Lerp(newDestination, biasPoint, _laneAlignmentBias);
        }
        return newDestination;
    }*/

    /*
    // Get a destination from a waypoint. Works with 2D box colliders only, which waypoints should have.
    private Vector2 GetNewDestination()
    {
        Vector2 newDestination = _waypointPath.WaypointList[_destinationIndex].GetRandomArea();

        if (_destinationIndex != 0)
        {
            // apply bias based on alignment of previous waypoint and current one
            Waypoint oldWaypoint = _waypointPath.WaypointList[_destinationIndex - 1];

            // Get vector to scale old destination with new waypoint's size
            float xBiasScale = _waypointPath.WaypointList[_destinationIndex].WaypointSize.x / oldWaypoint.WaypointSize.x;
            float yBiasScale = _waypointPath.WaypointList[_destinationIndex].WaypointSize.y / oldWaypoint.WaypointSize.y;
            // set bias point to local space then apply scales
            Vector2 biasPoint = _currentDestination - (Vector2)oldWaypoint.transform.position;
            biasPoint *= new Vector2(xBiasScale, yBiasScale);
            biasPoint += (Vector2)_waypointPath.WaypointList[_destinationIndex].transform.position;

            // apply bias to new destination
            newDestination = Vector2.Lerp(newDestination, biasPoint, _laneAlignmentBias);
        }
        return newDestination;
    }*/
}
