using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Collider2D collider1;
    public Collider2D collider2;
    // Start is called before the first frame update
    void Start()
    {
        print(collider1.bounds);
        print(collider2.bounds);
    }
}
