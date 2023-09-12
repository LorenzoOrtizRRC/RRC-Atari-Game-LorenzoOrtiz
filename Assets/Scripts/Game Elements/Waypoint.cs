using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Waypoint : MonoBehaviour
{
    public Collider2D WaypointArea => _waypointArea;
    public Vector2 WaypointSize => _waypointArea.bounds.size;
    public Vector2 WaypointCenter => _waypointArea.bounds.center;
    [SerializeField] private Collider2D _waypointArea;

    // gameobject must have a scale of 1 to be accurate
    public Vector2 GetRandomArea()
    {
        // get randomized area and return point. this works with any 2D collider.
        // extents are half of X & Y side lengths.
        float xExtents = _waypointArea.bounds.extents.x;
        float yExtents = _waypointArea.bounds.extents.y;
        float randomX = Random.Range(-xExtents, xExtents);
        float randomY = Random.Range(-yExtents, yExtents);
        return new Vector2(randomX, randomY) + (Vector2)transform.position;
    }
}
