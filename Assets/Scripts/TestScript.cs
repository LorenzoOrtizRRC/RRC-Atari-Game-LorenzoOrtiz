using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public float speed = 1f;
    public WaypointMovement movementState = new WaypointMovement();
    public Rigidbody2D rb;

    private void Start()
    {
        movementState.Initialize();
    }

    private void FixedUpdate()
    {
        movementState.MoveAgent(transform, rb, speed);
    }
}
