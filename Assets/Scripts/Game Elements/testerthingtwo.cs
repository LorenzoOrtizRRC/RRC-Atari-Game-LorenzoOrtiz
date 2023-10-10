using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testerthingtwo : MonoBehaviour
{
    public float speed;

    private void Start()
    {
        /*
        List<float> things = new() {0, 3, 2, 0.1f, -1f, -2f, -3f, -0.1f, -1.5f, 1.5f};
        things.Sort(ThingSorter);
        foreach (float thing in things) print(thing);*/
    }

    public void Move()
    {
        //
    }

    /*
    private static int ThingSorter(float x, float y)
    {
    // LIST SORTS IN ASCENDING ORDER, DEPENDING ON THE VALUE THE INT RETURNS (-1, 0, 1)
        return x.CompareTo(y);
    }*/
    /*
    public GameObject testerPrefab;
    private void OnEnable()
    {
        print("OnEnable");
    }
    private void Awake()
    {
        print("Awake");
    }
    private void Start()
    {
        print("Start");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) MakeTester();
    }

    public void TesterMethod()
    {
        Debug.LogWarning("PUBLIC METHOD");
    }

    public void MakeTester()
    {
        print("INSTANTIATE START");
        Instantiate(testerPrefab, Vector3.zero, Quaternion.identity).GetComponent<testerthingtwo>().TesterMethod();
        TesterMethod();
        print("INSTANTIATE END");
    }
    */
}
