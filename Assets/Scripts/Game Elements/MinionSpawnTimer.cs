using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionSpawnTimer
{
    private SpawnerData _spawnerData;
    private float _spawnTimer = 0f;
    private float _burstTimer = 0f;
    private bool _isSpawning = false;
    private bool _spawnInBursts = false;

    public void InitializeTimer(SpawnerData spawnerData)
    {
        _spawnerData = spawnerData;
    }

    public bool TickTimer(SpawnerData data)
    {
        if (Time.time >= _spawnTimer) return true;
        return false;
    }

    public void ResetTimer(float newSpawnRate, float newBurstRate)
    {
        _spawnTimer = Time.time + newSpawnRate;
        _burstTimer = newBurstRate;
    }
}
