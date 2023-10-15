using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class CharacterAgent : MonoBehaviour
{
    public UnityEvent<float> OnDamageTaken;     // float is mitigated damage taken
    public UnityEvent<float> OnHealthChanged;     // returned float is range 0 - 1. returned float is current health / max health
    public UnityEvent OnAgentDeath;
    //public Action<CharacterAgent> OnEnemyTargetAcquired;    // when character gets a new _enemyTarget

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
    [SerializeField] private bool _disableWeapon = false;
    [SerializeField] private bool _isInvulnerable = false;        // Cannot be damaged. Projectiles may still collide with this agent, but it will take no damage.
    [SerializeField] private bool _isUntargetable = false;      // Cannot be targeted. Only affects AI.
    [SerializeField] private bool _disableOnDeath = false;      // Disables itself instead of destroying on death.
    [SerializeField] private bool _healthBarVisible = true;     // Enable/Disable the health bar.
    [Header("Life Dependancies: Agent dies when the parents (CharacterAgent) its dependent on are all dead.")]
    [SerializeField] private bool _lifeIsDependent = false;
    [SerializeField] private bool _replaceDependencyTeams = true;
    [SerializeField] private List<CharacterAgent> _dependencyParentAgents = new List<CharacterAgent>();
    [SerializeField] private bool _addTeamColorsToEffects = true;
    [SerializeField] private ParticleSystem _deathEffect;

    //[SerializeField] private bool _cannotMove = false;
    /*
    [Header("State Machine Variables")]
    [SerializeField] private WeaponInstance _weapon;
    [SerializeField] private WaypointMovement _movementState = new WaypointMovement();
    [SerializeField] private MovementState _chaseState = new ChaseState();
    */

    private float _currentHealth;
    private bool _isDead = false;       // Used to check for the state of dependency agents.

    public Rigidbody2D Rb => _rb;
    public string AgentName => _stats.CharacterName;
    public float MaxHealth => _stats.Health;
    public float Armor => _stats.Armor;
    public float Speed => _stats.Speed;
    public float RotationSpeed => _stats.RotationSpeed;
    public float AggroRangeRadius => _stats.AggroRangeRadius;
    public WeaponInstance EquippedWeapon => _weapon;
    public TeamData CurrentTeam => _currentTeam;
    public bool IsInvincible => _isInvulnerable;
    public bool IsUntargetable => _isUntargetable;
    public List<CharacterAgent> DependencyParentAgents => _dependencyParentAgents;
    public float CurrentHealth => _currentHealth;
    public bool IsDead => _isDead;

    private float _healthRegenTimer = 0f;

    public void InitializeAgent(TeamData newTeam)
    {
        //_currentTeam = newTeam;
        SetTeam(newTeam);
    }

    private void Awake()
    {
        // initialize weapons and other components
        if (_healthBar && _healthBar.gameObject.activeInHierarchy)
        {
            if (_healthBarVisible)
            {
                OnHealthChanged.AddListener(_healthBar.UpdateSliderValue);
            }
            else _healthBar.gameObject.SetActive(false);
        }
        //_targetDetector.InitializeTargetDetector(_currentTeam);
        // initialize variables
        if (!_lifeIsDependent || _dependencyParentAgents == null || _dependencyParentAgents.Count == 0)
        {
            _lifeIsDependent = false;
        }
        else
        {
            foreach (CharacterAgent dependencyParent in _dependencyParentAgents)
            {
                dependencyParent.OnAgentDeath.AddListener(EvaluateLifeDependencies);
                if (_replaceDependencyTeams) dependencyParent.SetTeam(_currentTeam);
            }
        }
        // initialize own events
        //OnEnemyTargetAcquired += _weapon.SetNewTarget;
    }

    private void OnEnable()
    {
        // initialize variables
        _currentHealth = MaxHealth;
        _isDead = false;
        // Initialize components
        _healthBar.UpdateSliderValue(_currentHealth);
        _characterArtController.Initialize(_currentTeam);
        _weapon.InitializeWeapon(_currentTeam);

        _healthRegenTimer = Time.time + _stats.HealthRegenRate;
    }

    private void Start()
    {
        //_characterArtController.Initialize(_currentTeam);
        //_weapon.InitializeWeapon(_currentTeam);

        // initialize dependency teams
        // this is done twice right now because target detector won't update its team correctly. need to replace with owner agent var instead.
        if (!_lifeIsDependent || _dependencyParentAgents == null || !_dependencyParentAgents.Any())
        {
            _lifeIsDependent = false;
        }
        else
        {
            foreach (CharacterAgent dependencyParent in _dependencyParentAgents)
            {
                dependencyParent.OnAgentDeath.AddListener(EvaluateLifeDependencies);
                if (_replaceDependencyTeams) dependencyParent.SetTeam(_currentTeam);
            }
        }
    }

    private void Update()
    {
        if (_stats.HealthRegenRate > 0 && Time.time >= _healthRegenTimer)
        {
            HealCharacter(_stats.HealthRegenAmount);
            _healthRegenTimer = Time.time + _stats.HealthRegenRate;
        }
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
        if (!_disableWeapon || !_weapon) _weapon.UseWeaponAuto(direction);
    }

    public void RotateWeapon(Vector3 direction)
    {
        if (!_disableWeapon || !_weapon) _weapon.RotateWeapon(direction);
    }

    public void SetTeam(TeamData newTeam)
    {
        _currentTeam = newTeam;
        _characterArtController.Initialize(newTeam);
        _weapon.InitializeWeapon(_currentTeam);
    }

    public void ToggleInvulnerable(bool isInvulnerable) => _isInvulnerable = isInvulnerable;

    public void ToggleUntargetable(bool isUntargetable) => _isUntargetable = isUntargetable;

    private void EvaluateLifeDependencies()
    {
        // True when at least 1 dependency is alive.
        //bool dependencyIsAlive = false;
        for (int i = 0; i < _dependencyParentAgents.Count; i++)
        {
            CharacterAgent dependencyAgent = _dependencyParentAgents[i];
            if (dependencyAgent && dependencyAgent.gameObject.activeInHierarchy && !dependencyAgent.IsDead)
            {
                //dependencyIsAlive = true;
                //break;
                return;
            }
        }
        //if (!dependencyIsAlive) KillCharacter();
        KillCharacter();
        //print(dependencyIsAlive);
    }

    public void HealCharacter(float healValue)
    {
        _currentHealth = Mathf.Min(_currentHealth + healValue, MaxHealth);
        OnHealthChanged?.Invoke(CurrentHealth / MaxHealth);
    }

    public void DamageCharacter(float rawDamage, bool bypassInvincibility = false)
    {
        if (!_isInvulnerable || bypassInvincibility)
        {
            // Damage formula.
            float mitigatedDamage = Mathf.Clamp(rawDamage - Armor, 1f, 999999f);
            //if (mitigatedDamage <= 0f) print($"MITIGATED DAMAGE IS <= 0f. NAME: {gameObject.name}, MITIGATED DAMAGE: {mitigatedDamage}");
            _currentHealth = Mathf.Clamp(_currentHealth - mitigatedDamage, 0f, MaxHealth);
            OnDamageTaken?.Invoke(mitigatedDamage);
            OnHealthChanged?.Invoke(CurrentHealth / MaxHealth);
        }
        // evaluate health.
        if (_currentHealth == 0) KillCharacter();
    }

    private void KillCharacter()
    {
        _isDead = true;
        if (_deathEffect)
        {
            ParticleSystem.MainModule deathParticles = Instantiate(_deathEffect, transform.position, Quaternion.identity).main;
            if (_addTeamColorsToEffects) deathParticles.startColor = _currentTeam.TeamColor;
        }

        if (_disableOnDeath) gameObject.SetActive(false);
        else Destroy(gameObject);
        OnAgentDeath?.Invoke();
    }
}
