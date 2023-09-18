using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    // NPC AI responsible for logic. Uses CharacterAgent to execute logic.
    [Header("References")]
    [SerializeField] private CharacterAgent _agent;
    [Header("State Machine Variables")]
    [SerializeField] private WaypointMovement _movementState = new WaypointMovement();
    [SerializeField] private MovementState _chaseState = new ChaseState();

    public void InitializeStateMachine(TeamData newTeam, List<Waypoint> initialPath)
    {
        _agent.InitializeAgent(newTeam);
        _movementState.SetWaypoints(initialPath);
        _movementState.Initialize();
    }

    private void Update()
    {
        //  if target is valid, evaluate target
        if (_agent.EnemyTarget)
        {
            if (!_agent.EnemyTarget.gameObject.activeSelf)    // if target is dead, reset detector (to check ontrigger again) and current enemy target.
            {
                _agent.ResetTarget();
                return;
            }
            // evaluate weapon ranges, chase or retreat appropriately
            float distanceFromEnemy = (_agent.EnemyTarget.transform.position - transform.position).magnitude;
            //if (distanceFromEnemy > _agent.EquippedWeapon.MaximumRange && !_agent.) _chaseState.MoveAgent(transform, _rb, Speed, _enemyTarget.transform.position);    // chase when out of range
            if (distanceFromEnemy < _agent.EquippedWeapon.MinimumRange) { /* put retreat state here */ }     // retreat when target is too close
            else _agent.UseWeapon();   // use weapon when within appropriate range
        }
        else MoveCharacter();
    }

    private void MoveCharacter()
    {
        _movementState.MoveAgent(transform, _agent.Rb, _agent.Speed);
    }
}
