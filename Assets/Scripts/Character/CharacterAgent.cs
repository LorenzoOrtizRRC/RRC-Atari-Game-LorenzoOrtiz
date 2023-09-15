using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public enum Team { cat = 0, dog = 1 }

public class CharacterAgent : MonoBehaviour
{
    // This class is in charge of managing the instance of the unit in the level, including tracking its live stats.
    [Header("Component References")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private TargetDetector _targetDetector;
    [Header("Agent Variables")]
    [SerializeField] private CharacterData _stats;
    [SerializeField] private Team _currentTeam = Team.cat;
    [SerializeField] private float _enemyDetectionRadius = 10f;
    [Header("State Machine Variables")]
    [SerializeField] private WeaponInstance _weapon;
    [SerializeField] private WaypointMovement _movementState;
    //[SerializeField] private MovementState _chaseState;

    private float _currentHealth;
    private CharacterAgent _enemyTarget;

    public float MaxHealth => _stats.Health;
    public float Armor => _stats.Armor;
    public float Speed => _stats.Speed;
    public Team CurrentTeam => _currentTeam;
    public float CurrentHealth => _currentHealth;

    private void Awake()
    {
        // initialize weapons and other components
        _targetDetector.InitializeTargetDetector(_currentTeam, _enemyDetectionRadius);
        _weapon.InitializeWeapon(_currentTeam);
        // initialize events
        _targetDetector.OnEnemyDetected += RegisterNewEnemy;
    }

    private void OnEnable()
    {
        // initialize variables
        _currentHealth = MaxHealth;
        _movementState.Initialize();
    }

    private void Update()
    {
        //  chase and attack enemy targets, else continue movement
        if (_enemyTarget && _enemyTarget.gameObject.activeSelf) UseWeapon();
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
        _movementState.MoveAgent(transform, _rb, Speed);
    }

    private void UseWeapon()
    {
        _weapon.UseWeapon();
    }

    private void RegisterNewEnemy(CharacterAgent enemyAgent)
    {
        _enemyTarget = enemyAgent;
    }

    private void DamageCharacter(float rawDamage)
    {
        // Damage formula.
        float mitigatedDamage = Mathf.Clamp(rawDamage - Armor, 1f, Mathf.Infinity);
        _currentHealth = Mathf.Clamp(_currentHealth - mitigatedDamage, 0f, MaxHealth);
    }

    private void KillCharacter()
    {
        gameObject.SetActive(false);
    }
}
