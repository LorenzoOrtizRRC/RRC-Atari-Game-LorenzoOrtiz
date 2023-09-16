using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _minionSpawn;       // minion to spawn. must be NPCs, and have CharacterAgent scripts.
    [SerializeField] private TeamData _spawnerTeam;
    [SerializeField] private List<Waypoint> _minionWaypoints;
    [SerializeField] private float _spawnRate = 1f;     // minions per second. formula: _spawnRate = 1f / minions-per-second
    [SerializeField] private Vector2 _spawnArea = Vector2.zero;

    private float _spawnTimer = 0f;

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
        Instantiate(_minionSpawn, GetSpawnPosition(), transform.rotation).TryGetComponent(out CharacterAgent agent);
        agent.InitializeAgent(_spawnerTeam, _minionWaypoints);
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