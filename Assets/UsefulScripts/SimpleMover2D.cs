using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is a simple utility script to make things move or rotate each frame 
public class SimpleMover2D : MonoBehaviour
{
    public bool isRotationActive = false;
    public float rotationSpeed = 0;

    public bool isMovementActive = false;
    public Vector2 moveSpeed = Vector2.zero;


    // Update is called once per frame
    void Update()
    {
        if (isMovementActive)
        {
            transform.Translate(moveSpeed * Time.deltaTime);
        }

        if (isRotationActive)
        {
            transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
        }
    }
}
