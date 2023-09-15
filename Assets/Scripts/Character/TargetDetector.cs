using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : MonoBehaviour
{
    public Action<CharacterAgent> OnEnemyDetected;
    
    [Header("Enemy detection range is set by the CharacterAgent class.")]
    [SerializeField] private CircleCollider2D _detectorCollider;
    [SerializeField] private float _enemyDetectionRadius = 10f;
    private Team ownerTeam = Team.cat;

    private void Awake()
    {
        _detectorCollider ??= GetComponent<CircleCollider2D>();
        _detectorCollider.radius = _enemyDetectionRadius;
    }

    public void InitializeTargetDetector(Team newTeam)
    {
        ownerTeam = newTeam;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out CharacterAgent collidingAgent))
        {
            if (collidingAgent.CurrentTeam != ownerTeam) OnEnemyDetected(collidingAgent);
        }
    }
}
