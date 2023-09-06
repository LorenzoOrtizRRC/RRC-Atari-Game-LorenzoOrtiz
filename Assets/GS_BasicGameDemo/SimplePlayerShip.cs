using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayerShip : MonoBehaviour
{
    // Shared
    public Vector2 inputVector; // 2D Vector, access with inputVector.x/y

    // Kinematic Settings
    public float moveSpeed = 10f; // don't forget the f for floats
    public float rotationSpeed = 4f;

    // Physics Settings
    public float forceAmount = 10f;
    public float torqueAmount = 4f;

    // Control Vars

    public Rigidbody2D rigid;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>(); 
    }

    // Update is called once per frame
    void Update()
    {
        // Cache Input
        inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    void FixedUpdate()
    {
        // Move forward (in local space) using physics. 
        rigid.AddRelativeForce(new Vector3(0, inputVector.y, 0) * forceAmount);
        rigid.AddTorque(inputVector.x * torqueAmount * -1f); // rotate on Z axis using torque
    }
}
