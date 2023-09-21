using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    // damage number ui element will be displayed in world space.
    [SerializeField] private float _lifetime = 2f;
    [Header("Movement")]
    [SerializeField] private bool _isStatic;    // if damage number moves
    [SerializeField] private float _movementSpeed;

    private float _lifetimeTimer;
    private Vector2 _movementDirection = Vector2.one;

    public void InitializeDamageNumber(Vector2 newDirection, bool isStatic = false)
    {
        _movementDirection = newDirection;
        _isStatic = isStatic;
    }

    private void OnEnable()
    {
        _lifetimeTimer = Time.time + _lifetime;
    }

    private void Update()
    {
        if (!_isStatic)
        {
            transform.Translate(_movementDirection * _movementSpeed * Time.deltaTime);
        }
        if (Time.time >= _lifetimeTimer) gameObject.SetActive(false);
    }
}
