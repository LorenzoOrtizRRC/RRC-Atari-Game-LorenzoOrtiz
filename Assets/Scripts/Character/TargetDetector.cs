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
    [SerializeField] private bool _showDetectionRadius = true;  // in editor only
    private TeamData ownerTeam;

    private void Awake()
    {
        _detectorCollider ??= GetComponent<CircleCollider2D>();
        _detectorCollider.radius = _enemyDetectionRadius;
    }

    public void InitializeTargetDetector(TeamData newTeam)
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

    private void OnDrawGizmos()
    {
        if (_showDetectionRadius)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _enemyDetectionRadius);
        }
    }
}
