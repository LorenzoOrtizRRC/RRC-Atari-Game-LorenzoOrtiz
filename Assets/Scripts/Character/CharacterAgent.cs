using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAgent : MonoBehaviour
{
    public Action<float> OnDamageTaken;     // float is mitigated damage taken
    public Action<float> OnHealthDecreased;     // returned float is range 0 - 1. returned float is current health / max health
    public Action<CharacterAgent> OnEnemyTargetAcquired;    // when character gets a new _enemyTarget

    // This class is in charge of managing the instance of the unit in the level, including tracking its live stats.
    [Header("Component References")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private CharacterArtController _characterArtController;
    [Header("UI Component References")]
    [SerializeField] private ResourceBar _healthBar;
    [Header("Agent Variables")]
    [SerializeField] private CharacterData _stats;
    [SerializeField] private TeamData _currentTeam;
    [SerializeField] private WeaponInstance _weapon;
    [SerializeField] private bool _healthBarVisible = true;

    //[SerializeField] private bool _cannotMove = false;
    /*
    [Header("State Machine Variables")]
    [SerializeField] private WeaponInstance _weapon;
    [SerializeField] private WaypointMovement _movementState = new WaypointMovement();
    [SerializeField] private MovementState _chaseState = new ChaseState();
    */

    private float _currentHealth;

    public Rigidbody2D Rb => _rb;
    public float MaxHealth => _stats.Health;
    public float Armor => _stats.Armor;
    public float Speed => _stats.Speed;
    public TeamData CurrentTeam => _currentTeam;
    public WeaponInstance EquippedWeapon => _weapon;
    public float CurrentHealth => _currentHealth;

    public void InitializeAgent(TeamData newTeam)
    {
        _currentTeam = newTeam;
    }

    private void Start()
    {
        // initialize weapons and other components
        _weapon.InitializeWeapon(_currentTeam);
        _characterArtController.Initialize(_currentTeam);
        //_targetDetector.InitializeTargetDetector(_currentTeam);
        // initialize own events
        //OnEnemyTargetAcquired += _weapon.SetNewTarget;
        // initialize component events
        //_targetDetector.OnEnemyDetected += RegisterNewEnemy;
        if (_healthBarVisible) OnHealthDecreased += _healthBar.UpdateSliderValue;
    }

    private void OnEnable()
    {
        // initialize variables
        _currentHealth = MaxHealth;
        _healthBar.UpdateSliderValue(_currentHealth);
        //_movementState.Initialize();
        //_chaseState.Initialize();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out ProjectileInstance collidingProjectile))
        {
            if (collidingProjectile.CurrentTeam != _currentTeam) DamageCharacter(collidingProjectile.Damage);
        }
    }
    /*
     * THIS FUNCTION WILL BE ENABLED IN THE FUTURE FOR PHYSICS (FORCE) MOVEMENT. FOR NOW AI AGENTS WILL MOVE FROM STATEMACHINE (REFERENCE OF RIGIDBODY2D IN THERE TOO).
    public void MoveCharacter()
    {
        if (_cannotMove) return;
        _movementState.MoveAgent(transform, _rb, Speed);
    }
    */
    public void UseWeapon(CharacterAgent enemyAgent)
    {
        //_weapon.UseWeaponAuto(enemyAgent);
    }

    public void UseWeapon(Vector2 direction)
    {
        _weapon.UseWeaponAuto(direction);
    }

    public void RotateWeapon(Vector3 direction)
    {
        _weapon.RotateWeapon(direction);
    }

    private void DamageCharacter(float rawDamage)
    {
        // Damage formula.
        float mitigatedDamage = Mathf.Clamp(rawDamage - Armor, 1f, Mathf.Infinity);
        _currentHealth = Mathf.Clamp(_currentHealth - mitigatedDamage, 0f, MaxHealth);

        OnDamageTaken?.Invoke(mitigatedDamage);
        OnHealthDecreased?.Invoke(CurrentHealth / MaxHealth);

        // evaluate health.
        if (_currentHealth == 0) KillCharacter();
    }

    private void KillCharacter()
    {
        //gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
