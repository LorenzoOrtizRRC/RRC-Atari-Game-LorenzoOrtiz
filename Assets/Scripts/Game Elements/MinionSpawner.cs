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
    [SerializeField] private List<SpawnerData> _spawnerDataList = new List<SpawnerData>();
    [SerializeField] private TeamData _spawnerTeam;
    [SerializeField] private List<Waypoint> _minionWaypoints;
    [SerializeField] private float _spawnRate = 1f;     // minions per second. formula: _spawnRate = 1f / minions-per-second
    [SerializeField] private Vector2 _spawnArea = Vector2.zero;

    private float _spawnTimer = 0f;
    private bool _isSpawning = false;

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
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, _spawnArea);
    }
}
