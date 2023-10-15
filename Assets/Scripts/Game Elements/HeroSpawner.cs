using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _heroToSpawn;
    [SerializeField] private TeamData _spawnerTeam;
    [SerializeField] private WaypointPath _minionPath;
    [SerializeField] private float _respawnTime = 30f;   // real seconds

    public TeamData SpawnerTeam => _spawnerTeam;

    private CharacterAgent _heroAgent;
    private float _respawnTimer = 0f;     // timer for waves

    private void OnEnable()
    {
        _respawnTimer = 0f;
    }

    private void Update()
    {
        if (!_heroAgent || !_heroAgent.gameObject.activeInHierarchy || _heroAgent.IsDead)
        {
            if (Time.time >= _respawnTimer)
            {
                _heroAgent = SpawnHero();
                _heroAgent.OnAgentDeath.AddListener(ResetTimer);
            }
        }
    }

    private void ResetTimer()
    {
        _respawnTimer = Time.time + _respawnTime;
    }

    private CharacterAgent SpawnHero()
    {
        if (!_heroToSpawn) return null;
        GameObject spawnedHero = Instantiate(_heroToSpawn, transform.position, Quaternion.identity);
        if (spawnedHero.TryGetComponent(out StateMachine stateMachine)) stateMachine.InitializeStateMachine(_spawnerTeam, _minionPath);
        return spawnedHero.GetComponent<CharacterAgent>();
    }
}
