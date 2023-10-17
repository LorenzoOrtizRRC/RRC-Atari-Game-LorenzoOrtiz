using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    [SerializeField] private List<Waypoint> _waypointList = new List<Waypoint>(2);
    [SerializeField] private bool _previewPath = false;
    [SerializeField] private float _previewPathEdgesRadius = 2f;
    public List<Waypoint> WaypointList => _waypointList;

    private void OnDrawGizmos()
    {
        if (_previewPath)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_waypointList.First().transform.position, _previewPathEdgesRadius);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_waypointList.Last().transform.position, _previewPathEdgesRadius);
            Gizmos.color = Color.yellow;
            for (int i = 0; i < _waypointList.Count; i++)
            {
                if (i == 0) continue;
                Gizmos.DrawLine(_waypointList[i - 1].transform.position, _waypointList[i].transform.position);
            }
        }
    }
}
