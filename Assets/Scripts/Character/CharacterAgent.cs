using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CharacterAgent : MonoBehaviour
{
    public UnityEvent<float> OnDamageTaken;     // float is mitigated damage taken
    public UnityEvent<float> OnHealthDecreased;     // returned float is range 0 - 1. returned float is current health / max health
    public UnityEvent OnAgentDeath;
    public Action<CharacterAgent> OnEnemyTargetAcquired;    // when character gets a new _enemyTarget

    // This class is in charge of managing the instance of the unit in the level, including tracking its live stats.
    [Header("Component References")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private CharacterArtController _characterArtController;
    [SerializeField] private WeaponInstance _weapon;
    [Header("UI Component References")]
    [SerializeField] private ResourceBar _healthBar;
    [Header("Agent Variables")]
    [SerializeField] private CharacterData _stats;
    [SerializeField] private TeamData _currentTeam;
    [SerializeField] private bool _isInvincible = false;        // Cannot be damaged. Projectiles may still collide with this agent, but it will take no damage.
    [SerializeField] private bool _isUntargetable = false;      // Cannot be targeted. Only affects AI.
    [SerializeField] private bool _disableOnDeath = false;      // Disables itself instead of destroying on death.
    [SerializeField] private bool _healthBarVisible = true;     // Enable/Disable the health bar.
    [Header("Life Dependancies: Agent dies when the parents (CharacterAgent) its dependent on are all dead.")]
    [SerializeField] private bool _lifeIsDependent = false;
    [SerializeField] private List<CharacterAgent> _dependencyParentAgents = new List<CharacterAgent>();

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
    public float AggroRange => _stats.AggroRange;
    public WeaponInstance EquippedWeapon => _weapon;
    public TeamData CurrentTeam => _currentTeam;
    public bool IsInvincible => _isInvincible;
    public bool IsUntargetable => _isUntargetable;
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
        if (_healthBar)
        {
            if (_healthBarVisible) OnHealthDecreased.AddListener(_healthBar.UpdateSliderValue);
            else _healthBar.gameObject.SetActive(false);
        }
        //_targetDetector.InitializeTargetDetector(_currentTeam);
        // initialize variables
        if (!_lifeIsDependent || _dependencyParentAgents == null || _dependencyParentAgents.Count == 0) _lifeIsDependent = false;
        else
        {
            foreach (CharacterAgent dependencyParent in _dependencyParentAgents)
            {
                dependencyParent.OnAgentDeath.AddListener(EvaluateLifeDependencies);
            }
        }
        // initialize own events
        //OnEnemyTargetAcquired += _weapon.SetNewTarget;
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

    private void EvaluateLifeDependencies()
    {
        // True when at least 1 dependency is alive.
        bool dependencyIsAlive = false;
        for (int i = 0; i < _dependencyParentAgents.Count; i++)
        {
            CharacterAgent dependencyAgent = _dependencyParentAgents[i];
            if (dependencyAgent || dependencyAgent.gameObject.activeSelf)
            {
                dependencyIsAlive = true;
                break;
            }
        }
        if (!dependencyIsAlive) KillCharacter();
    }

    private void DamageCharacter(float rawDamage, bool bypassInvincibility = false)
    {
        float mitigatedDamage = 0;
        if (!_isInvincible || bypassInvincibility)
        {
            // Damage formula.
            mitigatedDamage = Mathf.Clamp(rawDamage - Armor, 1f, Mathf.Infinity);
            _currentHealth = Mathf.Clamp(_currentHealth - mitigatedDamage, 0f, MaxHealth);
        }
        OnDamageTaken?.Invoke(mitigatedDamage);
        OnHealthDecreased?.Invoke(CurrentHealth / MaxHealth);
        // evaluate health.
        if (_currentHealth == 0) KillCharacter();
    }

    private void KillCharacter()
    {
        if (_disableOnDeath) gameObject.SetActive(false);
        else Destroy(gameObject);
    }
}
