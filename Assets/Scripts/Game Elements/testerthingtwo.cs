using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testerthingtwo : MonoBehaviour
{
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
}
