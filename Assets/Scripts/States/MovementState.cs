using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementState
{
    public abstract void MoveAgent(Transform self, float speed);
}
