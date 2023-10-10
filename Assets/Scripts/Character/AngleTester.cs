using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleTester : MonoBehaviour
{
    public Transform ObjectA;
    public Transform ObjectB;
    public float AngularSpeed = 1f;

    // Update is called once per frame
    void Update()
    {
        if (!ObjectA && !ObjectB) return;
        float angleDifference = Vector2.SignedAngle((Vector2)ObjectA.transform.position, (Vector2)ObjectB.transform.position - (Vector2)ObjectA.transform.position);
        //float targetAngle = Mathf.Abs(angleDifference);
        float angleLimit = Mathf.Sign(angleDifference) * 180f;
        float moveTowardAngle = Mathf.MoveTowardsAngle(angleDifference, angleLimit, AngularSpeed * Time.fixedDeltaTime);
        print($"Angle difference:{angleDifference}, MoveTowardAngle: {moveTowardAngle}");
    }
}
