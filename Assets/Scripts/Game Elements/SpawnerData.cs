using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spawner Data", menuName = "Game Elements/Spawner Data")]
public class SpawnerData : ScriptableObject
{
    [SerializeField] private GameObject _minionPrefab;
    [SerializeField] private bool _spawnInBursts = true;    // if false, spawns continuously
    [SerializeField] private float _spawnRate = 1f;     // spawns every (_spawnrate) in real seconds. starts after burst spawning finishes.
    [SerializeField] private float _burstSpawnRate = 1f;    // delay between spawns during burst spawning
    [SerializeField] private int _burstSpawnCount = 1;  // number of spawns during burst spawning
}
