using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterMover// : MonoBehaviour
{
    [Header("Movement Variables")]
    [SerializeField] private float _movementSpeed = 1f;
    [SerializeField] private float _rotationSpeed = 180f;

    private Vector2 _currentDirection = Vector2.up;     // (0, 1) is the forward direction. Current direction is cached for rotation purposes, since the actual game object doesn't rotate.

    // Returns destination point. By default returns forward (for weapon rotation purposes).
    public virtual Vector2 MoveAgent(Transform agentTransform, Rigidbody2D agentRigidbody, float speed, Vector2? customDirection = null)
    {
        // Move an agent towards a direction.
        if (!customDirection.HasValue) return agentTransform.forward;
        Vector2 newDirection = customDirection.Value;
        Vector2 destination = (Vector2)agentTransform.position + (newDirection.normalized * Time.fixedDeltaTime * speed);
        StartMovement(agentRigidbody, destination);
        return destination;
    }

    protected virtual void StartMovement(Rigidbody2D agentRigidbody, Vector2 destination)
    {
        // Actual logic for moving the agent goes here.
        // Steer agent towards target direction.
        // Move agent towards target point.
        agentRigidbody.MovePosition(destination);
    }
}
