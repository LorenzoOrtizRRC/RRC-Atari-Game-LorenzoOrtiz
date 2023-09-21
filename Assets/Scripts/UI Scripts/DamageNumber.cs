using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    // damage number ui element will be displayed in world space.
    [SerializeField] private TextMeshProUGUI _displayText;
    [SerializeField] private float _lifetime = 2f;
    [Header("Movement")]
    [SerializeField] private float _movementSpeed;

    private bool _isStatic;    // if damage number moves
    private float _lifetimeTimer;
    private Vector2 _movementDirection = Vector2.one;

    public void InitializeDamageNumber(float newDamageValue, Vector2 initialPosition, Vector2 newDirection, bool isStatic = false)
    {
        _displayText.text = newDamageValue.ToString();
        transform.position = initialPosition;
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
