using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GenericObjectPooler : MonoBehaviour
{
    public static GenericObjectPooler CurrentInstance;

    [SerializeField] private List<GameObject> _objectsToPool = new List<GameObject>();  // to initialize _objectPools
    [SerializeField] private int _initialObjectPoolSize = 10;   // initial number of objects to create in object pool

    private Dictionary<GameObject, List<GameObject>> _objectPools;

    // initialize
    private void OnEnable()
    {
        CurrentInstance = this;
        _objectPools = new Dictionary<GameObject, List<GameObject>>();

        if (_objectsToPool.Count == 0 || _initialObjectPoolSize == 0) return;
        for (int i = 0; i < _objectsToPool.Count; i++)
        {
            AddEmptyObjectPool(_objectsToPool[i]);
            for (int j = 0; j < _initialObjectPoolSize; j++) AddNewObjectToPool(_objectsToPool[i], _objectsToPool[i]);
        }
    }

    // reset on disable
    private void OnDisable()
    {
        CurrentInstance = null;
        _objectPools = new Dictionary<GameObject, List<GameObject>>();
    }

    public GameObject GetGameObjectFromPool(GameObject gameObjectToGet)
    {
        // if object pool exists, get an inactive object, else if no inactive object available, make a new gameObject and add to that object pool
        // an inactive object means it is available
        if (!gameObjectToGet) return null;
        if (_objectPools.ContainsKey(gameObjectToGet))
        {
            GameObject availableGameObject = _objectPools[gameObjectToGet].Find((x) => !x.activeSelf);
            if (availableGameObject)
            {
                // return this object
                return availableGameObject;
            }
            else
            {
                // make a new copy if no objects are active/available
                return AddNewObjectToPool(gameObjectToGet, gameObjectToGet);
            }
        }
        else
        {
            AddEmptyObjectPool(gameObjectToGet);
            return AddNewObjectToPool(gameObjectToGet, gameObjectToGet);
        }
    }

    private void AddEmptyObjectPool(GameObject gameObjectKey) => _objectPools.Add(gameObjectKey, new List<GameObject>());

    private GameObject AddNewObjectToPool(GameObject gameObjectKey, GameObject gameObjectToAdd)
    {
        GameObject newObject = Instantiate(gameObjectToAdd, Vector3.zero, Quaternion.identity);
        newObject.SetActive(false);
        _objectPools[gameObjectKey].Add(newObject);
        return newObject;
    }
}
