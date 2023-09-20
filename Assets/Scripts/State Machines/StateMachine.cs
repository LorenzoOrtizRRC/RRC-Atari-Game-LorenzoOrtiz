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
    private CharacterAgent _enemyTarget;
    public CharacterAgent EnemyTarget => _enemyTarget;

    public void InitializeStateMachine(TeamData newTeam, List<Waypoint> initialPath)
    {
        _agent.InitializeAgent(newTeam);
        _movementState.SetWaypoints(initialPath);
        _movementState.Initialize();
    }

    private void Start()
    {
        // initialize components
        _targetDetector.InitializeTargetDetector(_agent.CurrentTeam);
        _targetDetector.OnEnemyDetected += RegisterNewEnemy;
    }

    private void Update()
    {
        Vector2 directionToMove = transform.forward;
        //  if target is valid, evaluate target
        if (_enemyTarget)
        {
            // replace 2nd condition with AGGRO RANGE from weapon data :>
            // if target is dead, reset detector (to check ontrigger again) and current enemy target.
            if (_enemyTarget.gameObject.activeSelf == false)
            {
                ResetTarget();
            }
            else
            {
                // evaluate weapon ranges, chase or retreat appropriately
                float distanceFromEnemy = (_enemyTarget.transform.position - transform.position).magnitude;
                // CHASE IS DISABLED CUZ OF NO AGGRO RANGE IMPLEMENTED IN WEAPONINSTANCE YET
                //if (distanceFromEnemy > _agent.EquippedWeapon.MaximumRange && !_agent.) _chaseState.MoveAgent(transform, _rb, Speed, _enemyTarget.transform.position);    // chase when out of range
                if (distanceFromEnemy < _agent.EquippedWeapon.MinimumRange) { /* put retreat state here */ }     // retreat when target is too close
                else
                {
                    //_agent.UseWeapon(_enemyTarget);   // use weapon when within appropriate range
                    _agent.UseWeapon(_enemyTarget.transform.position);
                }
            }
        }
        //else directionToMove = MoveCharacter();

        if (_enemyTarget) _agent.RotateWeapon(_enemyTarget.transform.position);
        //else _agent.RotateWeapon(directionToMove);
    }

    private void FixedUpdate()
    {
        Vector2 directionToMove = transform.forward;
        //  if target is valid, evaluate target
        if (!_enemyTarget)
        {
            directionToMove = MoveCharacter();
            _agent.RotateWeapon(directionToMove);
        }
        else _agent.RotateWeapon(_enemyTarget.transform.position);
    }

    private Vector2 MoveCharacter()
    {
        return _movementState.MoveAgent(transform, _agent.Rb, _agent.Speed);
    }

    private void RegisterNewEnemy(CharacterAgent enemyAgent)
    {
        if (_enemyTarget) return;
        _enemyTarget = enemyAgent;
        //OnEnemyTargetAcquired(_enemyTarget);
    }

    public void ResetTarget()
    {
        _enemyTarget = null;
        /*
        _targetDetector.gameObject.SetActive(false);
        _targetDetector.gameObject.SetActive(true);
        */
        _targetDetector.ResetDetector();
    }
}
