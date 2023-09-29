using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    [SerializeField] private List<Waypoint> _waypointList = new List<Waypoint>(2);
    public List<Waypoint> WaypointList => _waypointList;
}
