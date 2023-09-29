using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    [SerializeField] private List<Transform> _waypointList = new List<Transform>(2);
    public List<Transform> WaypointList => _waypointList;
}
