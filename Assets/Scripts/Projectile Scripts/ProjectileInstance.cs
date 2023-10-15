using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileInstance : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private CharacterArtController _characterArtController;
    [Header("Effects")]
    [SerializeField] private bool _enableHitEffect = true;
    [SerializeField] private ParticleSystem _hitEffect;
    [SerializeField] private bool _addTeamColorToEffects = true;
    [SerializeField] private bool _effectRotatesToHitDirection = true;
    [SerializeField, Range(0f, 1f)] private float _rotationToHitMultiplier = 0.2f;
    [SerializeField] private bool _effectInheritsProjectileDirection = false;

    private float _damage = 0f;
    private float _speed = 1f;
    private float _lifetime = 1f;
    private int _penetrationStrength = 0;        // Number of times the projectile can pierce through a target.
    private float _splashRadius = 0f;       // Splash damage.
    private LayerMask _splashMask;
    private TeamData _currentTeam;

    private float _elapsedLifetime = 0f;
    private int _penetrationCounter = 0;

    public float Damage => _damage;
    public TeamData CurrentTeam => _currentTeam;

    public void InitializeProjectile(TeamData newTeam, float newDamage, float newSpeed, float newLifetime, int newPenetrationStrength, float newSplashRadius, LayerMask newSplashMask)
    {
        _currentTeam = newTeam;
        _damage = newDamage;
        _speed = newSpeed;
        _lifetime = newLifetime;
        _penetrationStrength = newPenetrationStrength;
        _splashRadius = newSplashRadius;
        _splashMask = newSplashMask;
    }

    private void Start()
    {
        _characterArtController.Initialize(_currentTeam);   // this is at start instead of awake because the projectile needs to be initialized first to get the team, before the artcontroller gets intialized.
    }

    private void Update()
    {
        if (_elapsedLifetime >= _lifetime)
        {
            SpawnHitEffect();
            DoSplashDamage();
            DestroyProjectile();
        }
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
                SpawnHitEffect(collision);
            }

            DoSplashDamage();

            if (_penetrationCounter > _penetrationStrength) DestroyProjectile();
            else _penetrationCounter++;
        }
    }

    private void DoSplashDamage()
    {
        if (_splashRadius == 0f) return;
        List<Collider2D> agentsWithinRange = Physics2D.OverlapCircleAll(transform.position, _splashRadius, _splashMask).ToList();
        for (int i = 0; i < agentsWithinRange.Count; i++)
        {
            if (agentsWithinRange[i].TryGetComponent(out CharacterAgent agent) && agent.CurrentTeam != CurrentTeam)
            {
                agent.DamageCharacter(_damage);
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

    private void SpawnHitEffect()
    {
        if (!_enableHitEffect || !_hitEffect) return;
        Quaternion effectRotation = Quaternion.identity;
        /*if (_effectInheritsProjectileDirection)
        {
            effectRotation = transform.rotation;
        }*/
        effectRotation = transform.rotation;
        ParticleSystem effect = Instantiate(_hitEffect, transform.position, effectRotation);
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
