using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    //public Vector2 WaypointArea => _waypointArea;
    public Vector2 WaypointSize => _waypointArea;
    //public Vector2 WaypointCenter => _waypointArea.bounds.center;
    [SerializeField] private Vector2 _waypointArea;
    [SerializeField] private bool _previewWaypointArea = true;

    // gameobject must have a scale of 1 to be accurate
    public Vector2 GetRandomArea()
    {
        // get randomized area and return point. this works with any 2D collider.
        // extents are half of X & Y side lengths.
        //float xExtents = _waypointArea.bounds.extents.x;
        //float yExtents = _waypointArea.bounds.extents.y;
        float xExtents = _waypointArea.x / 2f;
        float yExtents = _waypointArea.y / 2f;
        float randomX = Random.Range(-xExtents, xExtents);
        float randomY = Random.Range(-yExtents, yExtents);
        return new Vector2(randomX, randomY) + (Vector2)transform.position;
    }

    private void OnDrawGizmos()
    {
        if (_previewWaypointArea)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, _waypointArea);
        }
    }
}
