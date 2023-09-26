using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.UI;

[CreateAssetMenu(fileName = "New Character Data", menuName = "Character/Character Data")]
public class CharacterData : ScriptableObject
{
    [SerializeField] private string _name = "Unit Name";
    [SerializeField] private float _health = 1f;
    [SerializeField] private float _armor = 0f;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _aggroRange = 1f;

    public string Name => _name;
    public float Health => _health;
    public float Armor => _armor;
    public float Speed => _speed;
    public float AggroRange => _aggroRange;
    /*
    [SerializeField] private Rigidbody2D _rb;

    [SerializeField] private float _maxHealth = 1f;
    [SerializeField] private float _moveSpeed = 1f;

    private float _hInput;
    private float _vInput;

    private void Start()
    {

        // initial checks
        if (!_rb) _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //
    }

    private void FixedUpdate()
    {
        // Move using physics.
        ReadPlayerInput();
        MoveCharacter(new Vector2(_hInput, _vInput));
    }

    public void ReadPlayerInput()
    {
        _hInput = (Input.GetAxis("Horizontal"));
        _vInput = (Input.GetAxis("Vertical"));

    }

    // Move based on delta time.
    public void MoveCharacter(Vector2 direction)
    {
        _rb.AddForce(direction * _moveSpeed * Time.deltaTime);
        Debug.Log(_rb.velocity.magnitude);
    }
    */
}
