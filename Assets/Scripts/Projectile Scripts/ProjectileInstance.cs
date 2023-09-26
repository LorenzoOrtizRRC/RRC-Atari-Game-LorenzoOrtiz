using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileInstance : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private CharacterArtController _characterArtController;
    private float _damage = 1f;
    private float _speed = 1f;
    private float _lifetime = 1f;
    private TeamData _currentTeam;

    private float _elapsedLifetime = 0f;

    public float Damage => _damage;
    public TeamData CurrentTeam => _currentTeam;

    public void InitializeProjectile(float newDamage, float newSpeed, float newLifetime, TeamData newTeam)
    {
        _damage = newDamage;
        _speed = newSpeed;
        _lifetime = newLifetime;
        _currentTeam = newTeam;
    }

    private void Awake()
    {
        _rb ??= GetComponent<Rigidbody2D>();
        _characterArtController ??= GetComponent<CharacterArtController>();
    }

    private void Start()
    {
        _characterArtController.Initialize(_currentTeam);   // this is at start instead of awake because the projectile needs to be initialized first to get the team, before the artcontroller gets intialized.
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out CharacterAgent collidingAgent))
        {
            if (collidingAgent.CurrentTeam != _currentTeam) DestroyProjectile();
        }
    }

    private void DestroyProjectile()
    {
        //gameObject.SetActive(false);
        Destroy(gameObject);
    }

}
