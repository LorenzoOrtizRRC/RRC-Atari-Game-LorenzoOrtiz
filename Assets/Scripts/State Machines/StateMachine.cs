using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StateMachine : MonoBehaviour
{
    // NPC AI responsible for logic. Uses CharacterAgent to execute logic.
    //public Action<CharacterAgent> OnEnemyTargetAcquired;    // when character gets a new _enemyTarget

    [Header("Component References")]
    [SerializeField] private CharacterAgent _agent;
    [SerializeField] private TargetDetector _targetDetector;
    [Header("State Machine Variables")]
    [SerializeField] private NPCMover _movementState = new NPCMover();
    [SerializeField] private MovementState _chaseState = new ChaseState();
    [SerializeField] private bool _isImmovable = false;
    [Header("Debug Gizmos")]
    [SerializeField] private bool _enableGizmos = false;
    [SerializeField] private bool _showObstacleAvoidance = false;
    [SerializeField] private bool _showAggroRange = false;
    [SerializeField] private bool _showDestinations = false;

    public CharacterAgent EnemyTarget => _enemyTarget;

    private CharacterAgent _enemyTarget;
    private bool _isChasing = false;

    private void OnEnable()
    {
        _movementState.Initialize(_agent.Speed, _agent.RotationSpeed, _agent);
    }

    public void InitializeStateMachine(TeamData newTeam, WaypointPath initialPath)
    {
        _agent.InitializeAgent(newTeam);
        //_movementState.SetWaypoints(initialPath);
        if (initialPath)
        {
            _movementState.Initialize(_agent.Speed, _agent.RotationSpeed, _agent, initialPath);
        }
        else
        {
            _movementState.Initialize(_agent.Speed, _agent.RotationSpeed, _agent);
        }
    }

    private void Start()
    {
        // initialize components
        if (_targetDetector && _targetDetector.gameObject.activeInHierarchy)
        {
            _targetDetector.InitializeTargetDetector(_agent.CurrentTeam);
            _targetDetector.OnEnemyDetected += RegisterNewEnemy;
        }

        // Set initial facing direction.
        if (_movementState.Destinations.Any())
        {
            Vector2 destinationDirection = _movementState.Destinations[0] - (Vector2)_agent.transform.position;
            _movementState.SetCurrentDirection(destinationDirection.normalized);
            _agent.EquippedWeapon.RotateWeaponInstant(destinationDirection.normalized);
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
            if (_enemyTarget.IsUntargetable || (!_enemyTarget.gameObject.activeInHierarchy || distanceToTarget > _agent.AggroRangeRadius))
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
        //else ResetTarget();
    }

    private void FixedUpdate()
    {
        Vector2 directionToMove = transform.forward;
        if (_enemyTarget)
        {
            //float distanceFromEnemy = (_enemyTarget.transform.position - transform.position).magnitude;
            if (_isChasing && !_isImmovable)
            {
                //_chaseState.MoveAgent(transform, _agent.Rb, _agent.Speed, _enemyTarget.transform.position);
                Vector2 directionToEnemy = _enemyTarget.transform.position - transform.position;
                _movementState.MoveAgent(transform, _agent.Rb, _agent.Speed, directionToEnemy);
            }
            _agent.RotateWeapon(_enemyTarget.transform.position);
        }
        else
        {
            if (_isImmovable) return;
            directionToMove = MoveCharacter();
            //_agent.RotateWeapon(directionToMove);
            _agent.RotateWeapon((Vector2)transform.position + _movementState.CurrentDirection);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // This fixes the problem of NPCs going beyond the waypoint, but coming back after somehow getting dragged beyond it.
        Waypoint currentWaypoint;
        if ((currentWaypoint = other.GetComponent<Waypoint>()) && currentWaypoint == _movementState.CurrentPath[_movementState.DestinationIndex]) _movementState.ForceIncrementDestinationIndex();
    }

    private Vector2 MoveCharacter()
    {
        return _movementState.MoveAgent(transform, _agent.Rb, _agent.Speed);
    }

    private void RegisterNewEnemy(CharacterAgent enemyAgent)
    {
        if (_enemyTarget && _enemyTarget.gameObject.activeInHierarchy) return;       // if target is still valid
        if (enemyAgent.IsUntargetable) return;      // if target cannot be targeted
        _enemyTarget = enemyAgent;
        // TRY TO REPLACE THIS WITH A BETTER IMPLEMENTATION WHEN OBJECT POOLING EXISTS PLS :)
        _enemyTarget.OnAgentDeath.AddListener(ResetTarget);
    }

    public void ResetTarget()
    {
        _enemyTarget = null;
        //_targetDetector.ResetDetector();
        //RaycastHit2D[] potentialTargets = Physics2D.CircleCastAll(transform.position, _targetDetector.EnemyDetectionRadius, Vector2.up, 0.1f);//, _targetDetector.DetectorLayerMask);
        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(_targetDetector.transform.position, _targetDetector.EnemyDetectionRadius);
        foreach (Collider2D target in potentialTargets)
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
        if (!_enableGizmos) return;
        // Obstacle Avoidance
        if (_showObstacleAvoidance)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, _movementState.CurrentDirection.normalized * 4f);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere((Vector2)transform.position, _movementState.AvoidanceRadius);
            Gizmos.DrawWireSphere((Vector2)transform.position + (_movementState.CurrentDirection.normalized * _movementState.AvoidanceCastLength), _movementState.AvoidanceRadius);
        }

        if (_showAggroRange)
        {
            // Visualize Aggro range.
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _agent.AggroRangeRadius);
        }

        if (_showDestinations)
        {
            // Visualize destinations.
            Gizmos.color = Color.green;
            if (_movementState.Destinations.Count > 1)
            {
                for (int i = _movementState.DestinationIndex; i < _movementState.Destinations.Count; i++)
                {
                    if (i >= _movementState.Destinations.Count) continue;
                    // Draw a line between the previous point and the current point.
                    Gizmos.DrawLine(_movementState.Destinations[i - 1], _movementState.Destinations[i]);
                }
            }
            if (_movementState.Destinations.Count > 0) Gizmos.DrawLine(transform.position, _movementState.Destinations[_movementState.DestinationIndex]);
        }
    }
}
