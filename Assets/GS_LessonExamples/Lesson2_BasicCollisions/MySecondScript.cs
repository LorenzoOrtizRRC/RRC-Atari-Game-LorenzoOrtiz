using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MySecondScript : MonoBehaviour
{
    public Vector2 inputVector; // 2D Vector, access with inputVector.x/y
    public float moveSpeed = 10f; // don't forget the f for floats
    public float forceAmount = 10f;

    public bool moveUsingKinematic = true;
    public bool moveUsingPhysics = true;

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

        // Won't Collide with anything (and stop moving) but can detect collisions. 
        if (moveUsingKinematic)
        {
                   // Apply movement directly to the Transform. 
            transform.Translate(inputVector * Time.deltaTime * moveSpeed); // The 2D Vector will get converted automatically to the 3D Vector the Transform needs.  

        }

    }

    void FixedUpdate()
    {
        if (moveUsingPhysics)
        {
            rigid.AddForce(inputVector * forceAmount);
        }

    }
}
