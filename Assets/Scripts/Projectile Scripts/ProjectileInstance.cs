using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileInstance : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private CharacterArtController _characterArtController;
    [Header("Effects")]
    [SerializeField] private bool _enableHitEffect = true;
    [SerializeField] private ParticleSystem _hitEffect;
    [SerializeField] private bool _addTeamColorToEffects = true;
    [SerializeField] private bool _effectRotatesToHitDirection = true;
    [SerializeField, Range(0f, 1f)] private float _rotationToHitMultiplier = 0.2f;
    [SerializeField] private bool _effectInheritsProjectileDirection = false;
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
            if (collidingAgent.CurrentTeam != _currentTeam)
            {
                //SpawnHitEffect(collision.GetContact(0).point);
                //SpawnHitEffect(collision.transform.position);
                SpawnHitEffect(collision);
                DestroyProjectile();
            }
        }
    }

    private void SpawnHitEffect(Collision2D collision)
    {
        if (!_enableHitEffect || !_hitEffect) return;
        Quaternion effectRotation = Quaternion.identity;
        if (_effectRotatesToHitDirection)
        {
            Vector2 targetDirection = (Vector2)collision.transform.position - (Vector2)transform.position;
            effectRotation = Quaternion.LookRotation(Vector3.forward, targetDirection);

            //float angle = Vector2.SignedAngle((Vector2)transform.position, direction);
            //effectRotation = Quaternion.AngleAxis(angle * _rotationToHitMultiplier, Vector3.forward);

            //effectRotation = Quaternion.LookRotation(direction, Vector3.forward);
        }
        else if (_effectInheritsProjectileDirection)
        {
            effectRotation = transform.rotation;
        }
        ParticleSystem effect = Instantiate(_hitEffect, collision.GetContact(0).point, effectRotation);
        if (_addTeamColorToEffects)
        {
            ParticleSystem.MainModule effectMainModule = effect.main;
            effectMainModule.startColor = _currentTeam.TeamColor;
        }
    }

    private void DestroyProjectile()
    {
        //gameObject.SetActive(false);
        Destroy(gameObject);
    }

}
