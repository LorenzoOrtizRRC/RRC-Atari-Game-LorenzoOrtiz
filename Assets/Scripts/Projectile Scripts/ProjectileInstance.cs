using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileInstance : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    private float _damage = 1f;
    private float _speed = 1f;
    private float _lifetime = 1f;
    private Team _currentTeam = Team.cat;

    private float _elapsedLifetime = 0f;

    public float Damage => _damage;
    public float Speed => _speed;
    public float Lifetime => _lifetime;

    public void InitializeProjectile(float newDamage, float newSpeed, float newLifetime, Team newTeam)
    {
        _damage = newDamage;
        _speed = newSpeed;
        _lifetime = newLifetime;
        _currentTeam = newTeam;
    }

    private void Start()
    {
        _rb ??= GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (_elapsedLifetime >= _lifetime) DestroyProjectile();
        _elapsedLifetime += Time.deltaTime;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        _rb.MovePosition(transform.position + (transform.up * _speed * Time.fixedDeltaTime));
    }

    private void DestroyProjectile()
    {
        gameObject.SetActive(false);
    }
}
