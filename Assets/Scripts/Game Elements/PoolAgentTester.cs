using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolAgentTester : MonoBehaviour
{
    public float lifetime = 5f;
    private float _timer;

    private void OnEnable()
    {
        _timer = lifetime;
    }

    private void Update()
    {
        if (_timer <= 0f) gameObject.SetActive(false);
        _timer -= Time.deltaTime;
    }
}
