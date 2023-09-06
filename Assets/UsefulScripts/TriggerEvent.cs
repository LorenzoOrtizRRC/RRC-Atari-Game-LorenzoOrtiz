using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    public UnityEvent triggerEvent;

    // Debugging
    public bool DEBUG_MODE = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Call the custom event (Added in Inspector), can call many methods here. 
        triggerEvent.Invoke();

        // DEBUG
        if (DEBUG_MODE) { Debug.Log("Trigger activated in GO " + gameObject.name + " by colliding with " + collision.gameObject.name); }
    }
}
