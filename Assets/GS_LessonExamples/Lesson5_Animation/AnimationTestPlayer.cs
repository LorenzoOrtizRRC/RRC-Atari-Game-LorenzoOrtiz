using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTestPlayer : MonoBehaviour
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
   
    public bool moveUsingPhysics = true;

    public bool moveInWorldSpace = true;
    public bool moveInLocalSpace = true;

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
        if (moveUsingPhysics)
        {
            if (moveInWorldSpace)
            {
                // Add movement using physics. Note we don't need Time.deltaTime
                // Uses World space by default
                rigid.AddForce(new Vector3(0, inputVector.y, 0) * forceAmount);
                rigid.AddTorque(inputVector.x * torqueAmount * -1f);
            }
            else if (moveInLocalSpace)
            {
                rigid.AddRelativeForce(new Vector3(0, inputVector.y, 0) * forceAmount);
                rigid.AddTorque(inputVector.x * torqueAmount * -1f);
            }

        }

    }
}
