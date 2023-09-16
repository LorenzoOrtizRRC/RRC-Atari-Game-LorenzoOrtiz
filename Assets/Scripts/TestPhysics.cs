using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPhysics : MonoBehaviour
{
    public Rigidbody2D rb;
    public ForceMode2D forceMode;
    public Vector2 forceAmount; // x only
    public bool applyForceOnce = false;
    // Start is called before the first frame update
    void Start()
    {
        if (applyForceOnce)
        {
            rb.AddForce(forceAmount, forceMode);
        }
        float baseNaturalLogarithm = 2.71828f;
        float maxVelocity = (forceAmount.x / rb.drag) * (1 - Mathf.Pow(baseNaturalLogarithm, -rb.drag / rb.mass));
        print($"max velocty: {maxVelocity}");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!applyForceOnce)
        {
            rb.AddForce(forceAmount, forceMode);
        }
    }
}
