using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class CharacterAgent : MonoBehaviour
{
    // This class is in charge of managing the instance of the unit in the level, including tracking its live stats.

    [SerializeField] private CharacterData _stats;
    [SerializeField] private Rigidbody2D _rb;
    [Header("State Machine Variables")]
    [SerializeField] private MovementState _movementState;
    [SerializeField] private MovementState _chaseState;
    [SerializeField] private List<WeaponInstance> _weapons;

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
        if (_enemyTarget) UseWeapons();
        else MoveCharacter();
    }

    private void MoveCharacter()
    {
        _movementState.MoveAgent(transform, _rb, Speed);
    }

    private void UseWeapons()
    {
        //
    }
}
