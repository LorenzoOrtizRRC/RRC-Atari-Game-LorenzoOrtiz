using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolerTester : MonoBehaviour
{
    public float _timerLength = 1f;
    public GameObject _objectToPool;

    private float _timer = 0f;

    private void Update()
    {
        if (Time.time >= _timer)
        {
            GameObject poolObj = GenericObjectPooler.SharedInstance.GetGameObjectFromPool(_objectToPool);
            poolObj.SetActive(true);
            _timer = Time.time + _timerLength;
        }
    }
}
