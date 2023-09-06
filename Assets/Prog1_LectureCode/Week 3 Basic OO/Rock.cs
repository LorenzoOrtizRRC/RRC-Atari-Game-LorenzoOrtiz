using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    // # Rock will have different sizes and a Sprite
    public int rockSizeLevel = 0;

    public Vector2 currentVelocity; 

    // # Add collision Detection
    // # when it hits something, break into a smaller rock. 

    public void Start()
    {
        // pick number between 1 and 3 (inclusive)
        rockSizeLevel = Random.Range(1, 4);
        
        if(rockSizeLevel == 1)
        {
            transform.localScale = new Vector2(.5f, .5f);
        }else if (rockSizeLevel == 2)
        {
            transform.localScale = new Vector2(1f, 1f);
        }
        else if (rockSizeLevel == 3)
        {
            transform.localScale = new Vector2(2f, 2f);
        }
        else
        {
            Debug.LogWarning("Rock Size of " + rockSizeLevel + " is Invalid ");
        }

        currentVelocity = Random.insideUnitCircle;
    }

    // Doing this for now, will change it to be controlled from the Spawner later.
    // (for reasons that will be clear when we get there so don't worry about it)
    public void Update()
    {
        transform.Translate(currentVelocity * Time.deltaTime);
    }
}
