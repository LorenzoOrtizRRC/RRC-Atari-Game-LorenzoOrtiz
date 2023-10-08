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
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _targetDistanceThreshold = 0.5f;
    [SerializeField] private float _steeringSpeedTest = 180f;
    [SerializeField] private float _avoidanceCastLength = 1f;
    [SerializeField] private float _avoidanceRadius = 1f;
    [SerializeField] private float _unstuckTimerDelay = 0.3f;       // When position is within _unstuckMinDistance of _lastPosition, timer will start. Unstucking starts when timer hits 0.
    [SerializeField] private float _unstuckMinDistance = 0.2f;      // When distance from _lastPosition is greater than this, _lastPosition will be reset.
    [SerializeField] private bool _enableUnstuck = true;            // Must be turned off for static/immovable agents.

    private List<Waypoint> _currentPath = new List<Waypoint>();     // This is used to generate the destination path. This is cached for reference after this path is finished.
    private List<Vector2> _destinations = new List<Vector2>();
    private int _destinationIndex = 0;

    private float unstuckTimer;
    private Vector2 _lastPosition;      // Used to unstuck self.

    public List<Waypoint> CurrentPath => _currentPath;
    public List<Vector2> Destinations => _destinations;
    public int DestinationIndex => _destinationIndex;

    //public void SetWaypoints(WaypointPath initialPath) => _waypointPath = initialPath;

    public void Initialize()
    {
        if (_currentPath.Any()) _currentPath = _initialPath.WaypointList;
        GenerateDestinations();
    }

    public void Initialize(WaypointPath newWaypointPath)
    {
        if (newWaypointPath.WaypointList.Any()) _currentPath = newWaypointPath.WaypointList;
        GenerateDestinations();
    }

    // Returns destination point. By default returns the transform local forward (for weapon rotation purposes).
    public override Vector2 MoveAgent(Transform agentTransform, Rigidbody2D agentRigidbody, float speed, Vector2? customDirection = null)
    {
        Vector2 directionToDestination = _destinations[_destinationIndex] - (Vector2)agentTransform.position;
        // Move if distance is greater than threshold, else get new waypoint.
        if (directionToDestination.sqrMagnitude >= _distanceThreshold * _distanceThreshold)
        {
            Vector2 destination = (Vector2)agentTransform.position + (directionToDestination.normalized * Time.fixedDeltaTime * speed);
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
            Debug.Log($"i: {i}");
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

    public void ForceIncrementDestinationIndex()
    {
        if (_destinationIndex < _destinations.Count - 1)
        {
            _destinationIndex++;
        }
    }
}
