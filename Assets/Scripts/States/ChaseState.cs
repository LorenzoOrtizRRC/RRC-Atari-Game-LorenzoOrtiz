using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class ChaseState : MovementState
{
    [SerializeField] private float _distanceThreshold = 0.5f;

    // move towards a destination
    public override Vector2 MoveAgent(Transform self, Rigidbody2D rb, float speed, Vector3? customDestination = null)
    {
        Vector2 direction = (customDestination ?? Vector3.zero) - self.transform.position;
        rb.MovePosition((Vector2)self.position + (direction.normalized * Time.deltaTime * speed));
        return Vector2.zero;
    }
}
