using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Spaceship : MonoBehaviour
{
    //  Note: We will make variables private later on, for now public is simpler
    // T1. 
    public Transform selfTransform;
    // T2 Input Cache - Let them figure out different ways. 
    public Vector2 input;
    // T3 Movement
    public float moveSpeed = 1f;

    // Reference to the Flaming Jet Trail
    // Using an actual object reference. 
    public SpriteRenderer jetRenderer;

    // Start is called before the first frame update
    void Start()
    {
        // Task 1. 
        selfTransform = transform;      
        // Really just a shortcut for GetComponent<Transform>() so we don't necessarilly want to call this every frame. 
    }

    // Update is called once per frame
    void Update()
    {
        // We can move in update because we are not using physics
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // No longer working because we are rotating the whole transform. 
        // selfTransform.Translate(input * Time.deltaTime * moveSpeed);

        // Now we can use up because the transform is being rotated, so basically
        // we are just saying drive forward (but forward i Z axis in unity which we do not use in 2d, so confusing!)
        // Up axis in this case is the "forward" of our ship. 
        // Alternatively we could do the rotation only on the art on a child transform, I will do this later. 
        // https://docs.unity3d.com/ScriptReference/Transform.Translate.html
        // Transform moves by default in its own orientation, that is, it uses its own forward value
        selfTransform.Translate(input.magnitude * new Vector2(0,1f) * Time.deltaTime * moveSpeed);

        // OR
        //selfTransform.Translate(input.magnitude * selfTransform.up * Time.deltaTime * moveSpeed, Space.World);

        Debug.Log("Self"+ selfTransform.up);

        // Update art
        RotateTowardsMovementDirection();

        FireJets();
    }

    void RotateTowardsMovementDirection()
    {
        // Get Ship forward direction 
        Vector2 up = selfTransform.up; // Shorthand for the Vector(1,0) direction

        // DrawLine and DrawRay - Show up in Scene but not Game, very useful!
        // Debugging - Draw Line draws from a position to another position
        Debug.DrawLine(transform.position, transform.position + (Vector3)up * 5f);
        // Draw ray draws from a position in a given direction, essentially the same as the above line in effect (except using input not up direction)
        Debug.DrawRay(transform.position, (Vector3)up * 5f);
        // Note the 5f is just an arbitrary multiplier value to make the lines longer. 

        // Take the angle between the forward direction and the input direction
        float signedAngle = Vector2.SignedAngle(up, input);
        
        // Add a rotation. This requires a vector 3 (so we create a new object) and pass the angle to the Z field. 
        selfTransform.Rotate(new Vector3(0,0, signedAngle));

        // More Debug
        // Debug.Log("Signed Angle between vectors: " + signedAngle);

    }

    void FireJets()
    {
        // Magnitude is just the length of a vector as a float (no direction)
        // So if the input is sufficient, turn the jet art on. 
        if (input.magnitude > .1f)
        {
            jetRenderer.enabled = true; // turns on the component
        }
        else
        {
            jetRenderer.enabled = false; // turns off the component
        }
    }
}
