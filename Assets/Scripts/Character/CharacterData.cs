using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterData : MonoBehaviour
{
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
}
