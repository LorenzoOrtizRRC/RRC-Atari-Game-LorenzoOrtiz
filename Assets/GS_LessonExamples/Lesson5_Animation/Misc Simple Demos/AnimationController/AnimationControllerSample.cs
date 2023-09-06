using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControllerSample : MonoBehaviour
{
    // This is the AnimationController, we control which animation is playing through this. 
    public Animator anim; 

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            anim.SetBool("ActivateKick", true);
        }

        // Triggers are like a bool but are "Consumed" when a transition "uses" them. 
        // They are basically a one shot bool. 
        // Confusingly named like the collider but has nothing to do with colliders. 
        // anim.SetTrigger("New Trigger");

        // Sets the float parameter
        // I like to just pipe the input directly into the animation system usually (for simple games at least). 

        // anim.SetInt("New Int", 5);
        // Don't forget the f for float values. 
        anim.SetFloat("New Float", Input.GetAxis("Vertical"));

    }
}
