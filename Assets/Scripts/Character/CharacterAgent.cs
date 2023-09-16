using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CharacterAgent : MonoBehaviour
{
    public Action<CharacterAgent> OnEnemyTargetAcquired;    // when character gets a new _enemyTarget

    // This class is in charge of managing the instance of the unit in the level, including tracking its live stats.
    [Header("Component References")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private CharacterArtController _characterArtController;
    [SerializeField] private TargetDetector _targetDetector;
    [Header("Agent Variables")]
    [SerializeField] private CharacterData _stats;
    [SerializeField] private TeamData _currentTeam;
    [SerializeField] private bool _cannotMove = false;
    [Header("State Machine Variables")]
    [SerializeField] private WeaponInstance _weapon;
    [SerializeField] private WaypointMovement _movementState = new WaypointMovement();
    [SerializeField] private MovementState _chaseState = new ChaseState();

    private float _currentHealth;
    private CharacterAgent _enemyTarget;

    public float MaxHealth => _stats.Health;
    public float Armor => _stats.Armor;
    public float Speed => _stats.Speed;
    public TeamData CurrentTeam => _currentTeam;
    public float CurrentHealth => _currentHealth;

    public void InitializeAgent(TeamData newTeam, List<Waypoint> initialPath)
    {
        _currentTeam = newTeam;
        _movementState.SetWaypoints(initialPath);
        _movementState.Initialize();
    }

    private void Start()
    {
        // initialize weapons and other components
        _weapon.InitializeWeapon(_currentTeam);
        _characterArtController.Initialize(_currentTeam);
        _targetDetector.InitializeTargetDetector(_currentTeam);
        // initialize own events
        OnEnemyTargetAcquired += _weapon.SetNewTarget;
        // initialize component events
        _targetDetector.OnEnemyDetected += RegisterNewEnemy;
    }

    private void OnEnable()
    {
        // initialize variables
        _currentHealth = MaxHealth;
        //_movementState.Initialize();
        //_chaseState.Initialize();
    }

    private void Update()
    {
        //  if target is valid, evaluate target
        if (_enemyTarget)
        {
            if (!_enemyTarget.gameObject.activeSelf)    // if target is dead, reset detector (to check ontrigger again) and current enemy target.
            {
                ResetTarget();
                return;
            }
            // evaluate weapon ranges, chase or retreat appropriately
            float distanceFromEnemy = (_enemyTarget.transform.position - transform.position).magnitude;
            if (distanceFromEnemy > _weapon.MaximumRange && !_cannotMove) _chaseState.MoveAgent(transform, _rb, Speed, _enemyTarget.transform.position);    // chase when out of range
            else if (distanceFromEnemy < _weapon.MinimumRange) { /* put retreat state here */ }     // retreat when target is too close
            else UseWeapon();   // use weapon when within appropriate range
        }
        else MoveCharacter();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out ProjectileInstance collidingProjectile))
        {
            if (collidingProjectile.CurrentTeam != _currentTeam) DamageCharacter(collidingProjectile.Damage);
        }
    }

    private void MoveCharacter()
    {
        if (_cannotMove) return;
        _movementState.MoveAgent(transform, _rb, Speed);
    }

    private void UseWeapon()
    {
        _weapon.UseWeapon(_enemyTarget);
    }

    private void RegisterNewEnemy(CharacterAgent enemyAgent)
    {
        _enemyTarget = enemyAgent;
        OnEnemyTargetAcquired(_enemyTarget);
    }

    private void ResetTarget()
    {
        _enemyTarget = null;
        _targetDetector.gameObject.SetActive(false);
        _targetDetector.gameObject.SetActive(true);
    }

    private void DamageCharacter(float rawDamage)
    {
        // Damage formula.
        float mitigatedDamage = Mathf.Clamp(rawDamage - Armor, 1f, Mathf.Infinity);
        _currentHealth = Mathf.Clamp(_currentHealth - mitigatedDamage, 0f, MaxHealth);

        // evaluate health.
        if (_currentHealth == 0) KillCharacter();
    }

    private void KillCharacter()
    {
        gameObject.SetActive(false);
    }
}
