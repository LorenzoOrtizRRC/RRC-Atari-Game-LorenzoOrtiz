using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MinionSpawner : MonoBehaviour
{
    /*private SpawnerData _spawnerData;
    public float _spawnTimer = 0f;
    public float _burstSpawnTimer = 0f;

    private bool _isBurstSpawning = false;

    private void Update()
    {
        if (Time.time >= _spawnTimer)
        {
            SpawnMinion();
            _spawnTimer = Time.time + _spawnRate;
        }
    }

    private void SpawnMinion()
    {
        //Instantiate(_minionSpawn, GetSpawnPosition(), Quaternion.identity).TryGetComponent(out StateMachine stateMachine);
        //stateMachine?.InitializeStateMachine(_spawnerTeam, _minionWaypoints);
    }

    private Vector2 GetSpawnPosition()
    {
        float xExtent = _spawnArea.x / 2f;
        float yExtent = _spawnArea.y / 2f;
        return new Vector2(Random.Range(-xExtent, xExtent), Random.Range(-yExtent, yExtent)) + (Vector2)transform.position;
    }*/

    //[SerializeField] private GameObject _minionSpawn;       // minion to spawn. must be NPCs, and have CharacterAgent scripts.
    [SerializeField] private List<GameObject> _minionWave = new List<GameObject>();      // minions to spawn per wave
    [SerializeField] private TeamData _spawnerTeam;
    [SerializeField] private List<Waypoint> _minionWaypoints;
    //[SerializeField] private float _spawnRate = 1f;     // minions per second. formula: _spawnRate = 1f / minions-per-second
    [SerializeField] private float _delayBetweenSpawnWaves = 10f;   // real seconds
    [SerializeField] private float _delayBetweenMinions = 1f;       // real seconds
    [SerializeField] private Vector2 _spawnArea = Vector2.zero;

    private float _waveTimer = 0f;     // timer for waves
    private float _minionSpawnTimer = 0f;       // timer for minions in waves
    private int _waveIndex = 0;
    private bool _isSpawningWave = false;

    private void Update()
    {/*
        if (Time.time >= _waveTimer)
        {
            SpawnMinion();
            _waveTimer = Time.time + _spawnRate;
        }*/
        if (Time.time >= _waveTimer) _isSpawningWave = true;
        if (_isSpawningWave)
        {
            if (Time.time >= _minionSpawnTimer)
            {
                SpawnMinion(_waveIndex);
                _minionSpawnTimer = Time.time + _delayBetweenMinions;
                _waveIndex++;
                if (_waveIndex > _minionWave.Count - 1)     // wave finished spawning.
                {
                    RefreshTimers();
                    _waveIndex = 0;
                    _isSpawningWave = false;
                }
            }
        }
        else if (Time.time >= _waveTimer) _isSpawningWave = true;
    }

    private void SpawnMinionWave()
    {
        //
    }

    private void SpawnMinion(int minionWaveIndex)
    {
        Instantiate(_minionWave[minionWaveIndex], GetSpawnPosition(), Quaternion.identity).TryGetComponent(out StateMachine stateMachine);
        stateMachine?.InitializeStateMachine(_spawnerTeam, _minionWaypoints);
    }

    private void RefreshTimers()
    {
        _waveTimer = Time.time + _delayBetweenSpawnWaves;
        _minionSpawnTimer = 0f;
    }

    private Vector2 GetSpawnPosition()
    {
        float xExtent = _spawnArea.x / 2f;
        float yExtent = _spawnArea.y / 2f;
        return new Vector2(Random.Range(-xExtent, xExtent), Random.Range(-yExtent, yExtent)) + (Vector2)transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, _spawnArea);
    }
}
