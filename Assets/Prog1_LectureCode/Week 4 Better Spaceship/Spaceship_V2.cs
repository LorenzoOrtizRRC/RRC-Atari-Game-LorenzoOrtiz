using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Spaceship_V2 : MonoBehaviour
{
    //  Note: We will make variables private later on, for now public is simpler
    // T1. 
    public Transform selfTransform;
    // T2 Input Cache - Let them figure out different ways. 
    public Vector2 input;
    // T3 Movement
    public float moveSpeed = 10f; // this is unity units per second.
    public float rotationSpeed = 165f; // rotation is much bigger because it is angle/second

    // Reference to the Flaming Jet Trail
    // SpriteRenderer components will let us enable and disable the flame asset directly.  
    public SpriteRenderer jetRenderer_Main;
    public SpriteRenderer jetRenderer_Left;
    public SpriteRenderer jetRenderer_Right;

    // Start is called before the first frame update
    void Start()
    {
        // No longer needed, I just wanted you to understand this.  
        // selfTransform = transform;      
        // Really just a shortcut for GetComponent<Transform>() so we don't necessarilly want to call this every frame. 
    }

    // Update is called once per frame
    void Update()
    {
        // We can move in update because we are not using physics
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        ManualControls();
        // ----- No longer being used -----
        // No longer working because we are rotating the whole transform. 
        // selfTransform.Translate(input * Time.deltaTime * moveSpeed);

        // Now we can use up because the transform is being rotated, so basically
        // we are just saying drive forward (but forward i Z axis in unity which we do not use in 2d, so confusing!)
        // Up axis in this case is the "forward" of our ship. 
        // Alternatively we could do the rotation only on the art on a child transform, I will do this later. 
        // https://docs.unity3d.com/ScriptReference/Transform.Translate.html
        // Transform moves by default in its own orientation, that is, it uses its own forward value
        //selfTransform.Translate(input.magnitude * new Vector2(0,1f) * Time.deltaTime * moveSpeed);
        // OR
        //selfTransform.Translate(input.magnitude * selfTransform.up * Time.deltaTime * moveSpeed, Space.World);
        // Update art
        //    RotateTowardsMovementDirection();
        // MainJets();
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

    private void ManualControls()
    {
        // Movement
        // input.y is a float = up/down arrows or w/s
        // transform.up is the vector direction relative to the ships current heading (local forward)
        // Time.deltaTime changes movespeed from distance per frame to difference per second (normalized over frame time)
        // moveSpeed is our control to change the speed when we press the button. 
        Vector3 moveVector = input.y * transform.up  * moveSpeed;
        // We need world space here because the transform.up is already giving us the forward
        // vector of the ship in world space (ie. it is not (0,1,0) all the time
        transform.Translate(moveVector * Time.deltaTime, Space.World);

        // Debug.Log(moveVector);
        // DrawLine and DrawRay are very useful for debugging vectors and movement
        Debug.DrawLine(transform.position, transform.position + moveVector * 200f);

        // Rotation: -1 is because the direction needs reversing. 
        float rotateVector = input.x * Time.deltaTime * rotationSpeed * -1f;
        transform.Rotate(0, 0, rotateVector); 

        // Movement Effects
        MainJets();
        // Rotation Effects
        RotationJets();
    }


    void MainJets()
    {
        // Magnitude is just the length of a vector as a float (no direction)
        // So if the input is sufficient, turn the jet art on. 
        if (input.y > .1f){
            // jetRenderer_Left.enabled = false;
            jetRenderer_Main.enabled = true; // turns on the component
        }else{
            jetRenderer_Main.enabled = false; // turns off the component
        }
        // No reverse jets/brake yet. 
        // [ ] Something to add. 
    }

    void RotationJets() {
        float rotationInput = input.x;
        if (rotationInput > 0.1f)
        {
            // Right?
            jetRenderer_Left.enabled = true;
            jetRenderer_Right.enabled = false;
        }
        else if(rotationInput < -0.1f)
        {
            jetRenderer_Left.enabled = false;
            jetRenderer_Right.enabled = true;
        }else
        {
            jetRenderer_Left.enabled = false;
            jetRenderer_Right.enabled = false;
        }
    }
}
