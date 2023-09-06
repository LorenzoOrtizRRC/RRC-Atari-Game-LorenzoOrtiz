using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFirstScript : MonoBehaviour
{
    // Step 1
    
    public float hInput;
    public float vInput;
    // Step 2
    public float moveSpeed = 10.5f; // Use f for floats. 

    // Update is called once per frame
    void Update()
    {   
        // -- Step 1---
        /*
                // Cache the Input variables. 
                hInput = Input.GetAxis("Horizontal");
                vInput = Input.GetAxis("Vertical");

                // First Draft
                transform.Translate(hInput, vInput, 0);
        */
        // ---Step 2 --- 
        
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");

        // Time.deltaTime is the frame time, AKA 1/FPS
        // Create a Vector3 
        Vector3 tempInputVector = new Vector3(hInput, vInput, 0);
        tempInputVector = tempInputVector * Time.deltaTime * moveSpeed;
        transform.Translate(tempInputVector);
        
        // Alt, could also use a Vector 3 Instead. 
        // I'll do that next module
    }
}
