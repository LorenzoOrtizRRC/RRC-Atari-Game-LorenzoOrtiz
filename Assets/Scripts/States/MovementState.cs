using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public abstract class MovementState
{
    public virtual void Initialize() { }
    // returns destination
    public abstract Vector2 MoveAgent(Transform self, Rigidbody2D rb, float speed, Vector3? customDestination = null);
}
