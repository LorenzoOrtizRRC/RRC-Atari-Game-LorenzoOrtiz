using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionSpawner : MonoBehaviour
{
    [SerializeField] private List<SpawnerData> _initialWave = new List<SpawnerData>();    // initial minions in starting waves
    [SerializeField] private TeamData _spawnerTeam;
    [SerializeField] private WaypointPath _minionPath;
    [SerializeField] private float _delayBetweenSpawnWaves = 10f;   // real seconds
    [SerializeField] private float _delayBetweenMinions = 1f;       // real seconds. delay between each minion spawn in a wave
    [SerializeField] private Vector2 _spawnArea = Vector2.zero;

    public TeamData SpawnerTeam => _spawnerTeam;

    private List<GameObject> _minionWave = new List<GameObject>();      // minions to spawn per wave
    private GameObject[] _currentMinionWave = null;      // Minions to spawn in current wave. This is so that changes to _minionWave don't affect the current wave being spawned.
    private float _waveTimer = 0f;     // timer for waves
    private float _minionSpawnTimer = 0f;       // timer for minions in waves
    private int _waveIndex = 0;
    private bool _isSpawningWave = false;

    private void Awake()
    {
        foreach (SpawnerData spawnerData in _initialWave)AddMinionsInWave(spawnerData);
    }

    private void Update()
    {
        if (_isSpawningWave)
        {
            // Spawn the minion wave.
            if (Time.time >= _minionSpawnTimer)
            {
                SpawnMinion(_waveIndex);
                _minionSpawnTimer = Time.time + _delayBetweenMinions;
                _waveIndex++;
                if (_waveIndex > _currentMinionWave.Length - 1)     // wave finished spawning.
                {
                    RefreshTimers();
                    _waveIndex = 0;
                    _isSpawningWave = false;
                }
            }
        }
        else if (Time.time >= _waveTimer)
        {
            // Start wave spawning if wave timer finishes.
            _currentMinionWave = _minionWave.ToArray();
            _isSpawningWave = true;
        }
    }

    public void AddMinionsInWave(SpawnerData spawnerData)
    {
        for (int i = 0; i < spawnerData.NumberOfMinions; i++)
        {
            _minionWave.Add(spawnerData.MinionPrefab);
        }
    }

    public void RemoveMinionsInWave(SpawnerData spawnerData)
    {
        for (int i = spawnerData.NumberOfMinions; i > 0; i--)
        {
            if (!_minionWave.Remove(spawnerData.MinionPrefab)) break;   // return if no more of element remains (for edge cases)
        }
    }

    private void SpawnMinion(int minionWaveIndex)
    {
        Instantiate(_currentMinionWave[minionWaveIndex], GetSpawnPosition(), Quaternion.identity).TryGetComponent(out StateMachine stateMachine);
        stateMachine?.InitializeStateMachine(_spawnerTeam, _minionPath);
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
