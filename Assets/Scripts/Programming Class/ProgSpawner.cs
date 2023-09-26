using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _prefabToSpawn;
    [SerializeField] private int _maxNumberOfObjects = 10;
    [SerializeField] private int _startingObjectsToSpawn = 10;
    [SerializeField] private float _spawnTime = 1f;

    private List<GameObject> _spawnedObjects = new List<GameObject>();
    private float _currentTimer = 0f;

    private void Start()
    {
        for (int i = 0; i < _maxNumberOfObjects; i++)
        {
            _spawnedObjects.Add(SpawnObject());
        }
    }

    private void Update()
    {
        if (Time.time >= _currentTimer && _spawnedObjects.Count < _maxNumberOfObjects)
        {
            _spawnedObjects.Add(SpawnObject());
            _currentTimer = Time.time + _spawnTime;
        }

        for (int i = 0; i < _spawnedObjects.Count; i++)
        {
            if (!_spawnedObjects[i]) _spawnedObjects.Remove(_spawnedObjects[i]);
        }
    }

    private GameObject SpawnObject()
    {
        return Instantiate(_prefabToSpawn);
    }
}
