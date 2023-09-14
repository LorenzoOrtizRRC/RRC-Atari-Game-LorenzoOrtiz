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
    [Header("Agent Variables")]
    [SerializeField] private CharacterData _stats;
    [SerializeField] private Team _currentTeam = Team.cat;
    [Header("State Machine Variables")]
    [SerializeField] private MovementState _movementState;
    [SerializeField] private MovementState _chaseState;
    [SerializeField] private WeaponInstance _weapon;

    private float _currentHealth;
    private CharacterAgent _enemyTarget;

    public float MaxHealth => _stats.Health;
    public float Armor => _stats.Armor;
    public float Speed => _stats.Speed;
    public float CurrentHealth => _currentHealth;


    private void Start()
    {
        _currentHealth = MaxHealth;
    }

    private void Update()
    {
        //  chase and attack enemy targets, else continue movement
        if (_enemyTarget) UseWeapon();
        else MoveCharacter();
    }

    private void MoveCharacter()
    {
        _movementState.MoveAgent(transform, _rb, Speed);
    }

    private void UseWeapon()
    {
        _weapon.UseWeapon(_currentTeam);
    }
}
