using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testerthingtwo : MonoBehaviour
{
    private void Start()
    {
        List<float> things = new() {0, 3, 2, 0.1f, -1f, -2f, -3f, -0.1f, -1.5f, 1.5f};
        things.Sort(ThingSorter);
        foreach (float thing in things) print(thing);
    }

    private static int ThingSorter(float x, float y)
    {
        return x.CompareTo(y);
    }
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
