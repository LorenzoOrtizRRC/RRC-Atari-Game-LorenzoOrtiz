using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateMachine : MonoBehaviour
{
    // NPC AI responsible for logic. Uses CharacterAgent to execute logic.
    public Action<CharacterAgent> OnEnemyTargetAcquired;    // when character gets a new _enemyTarget

    [Header("Component References")]
    [SerializeField] private CharacterAgent _agent;
    [SerializeField] private TargetDetector _targetDetector;
    [Header("State Machine Variables")]
    [SerializeField] private WaypointMovement _movementState = new WaypointMovement();
    [SerializeField] private MovementState _chaseState = new ChaseState();
    [SerializeField] private bool _isImmovable = false;

    public CharacterAgent EnemyTarget => _enemyTarget;

    private CharacterAgent _enemyTarget;
    private bool _isChasing = false;

    public void InitializeStateMachine(TeamData newTeam, WaypointPath initialPath)
    {
        _agent.InitializeAgent(newTeam);
        _movementState.SetWaypoints(initialPath);
        _movementState.Initialize();
    }

    private void Start()
    {
        // initialize components
        if (_targetDetector || !_targetDetector.gameObject.activeSelf)
        {
            _targetDetector.InitializeTargetDetector(_agent.CurrentTeam);
            _targetDetector.OnEnemyDetected += RegisterNewEnemy;
        }
    }

    private void Update()
    {
        Vector2 directionToMove = transform.forward;
        //  if target is valid, evaluate target
        if (_enemyTarget)
        {
            // replace 2nd condition with AGGRO RANGE from weapon data :>
            // if target is dead, reset detector (to check ontrigger again) and current enemy target.
            float distanceToTarget = (_enemyTarget.transform.position - transform.position).magnitude;
            if (!_enemyTarget.gameObject.activeInHierarchy || distanceToTarget > _agent.AggroRangeRadius)
            {
                ResetTarget();
            }
            else
            {
                // evaluate weapon ranges, chase or retreat appropriately
                float distanceFromEnemy = (_enemyTarget.transform.position - transform.position).magnitude;
                // CHASE IS DISABLED CUZ OF NO AGGRO RANGE IMPLEMENTED IN WEAPONINSTANCE YET
                if (distanceFromEnemy > _agent.EquippedWeapon.MaximumRange) _isChasing = true; //_chaseState.MoveAgent(transform, _agent.Rb, _agent.Speed, _enemyTarget.transform.position);    // chase when out of range
                //else if (distanceFromEnemy < _agent.EquippedWeapon.MinimumRange) { /* put retreat state here */ }     // retreat when target is too close
                else
                {
                    //_agent.UseWeapon(_enemyTarget);   // use weapon when within appropriate range
                    _agent.UseWeapon(_enemyTarget.transform.position);
                    _isChasing = false;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        Vector2 directionToMove = transform.forward;
        if (_enemyTarget)
        {
            float distanceFromEnemy = (_enemyTarget.transform.position - transform.position).magnitude;
            if (_isChasing && !_isImmovable) _chaseState.MoveAgent(transform, _agent.Rb, _agent.Speed, _enemyTarget.transform.position);
            _agent.RotateWeapon(_enemyTarget.transform.position);
        }
        else
        {
            if (_isImmovable) return;
            directionToMove = MoveCharacter();
            _agent.RotateWeapon(directionToMove);
        }
    }

    private Vector2 MoveCharacter()
    {
        return _movementState.MoveAgent(transform, _agent.Rb, _agent.Speed);
    }

    private void RegisterNewEnemy(CharacterAgent enemyAgent)
    {
        if (_enemyTarget) return;       // if target is still valid
        if (enemyAgent.IsUntargetable) return;      // if target cannot be targeted
        _enemyTarget = enemyAgent;
    }

    public void ResetTarget()
    {
        _enemyTarget = null;
        //_targetDetector.ResetDetector();
        RaycastHit2D[] potentialTargets = Physics2D.CircleCastAll(_targetDetector.transform.position, _targetDetector.EnemyDetectionRadius, Vector2.up, 0f);//, _targetDetector.DetectorLayerMask);
        foreach (RaycastHit2D target in potentialTargets)
        {
            CharacterAgent agent = target.transform.GetComponent<CharacterAgent>();
            if (agent && agent.CurrentTeam != _agent.CurrentTeam)
            {
                RegisterNewEnemy(agent);
                break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Visualize Aggro range.
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _agent.AggroRangeRadius);
    }
}
