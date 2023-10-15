using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileInstance : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private CharacterArtController _characterArtController;
    //[SerializeField] private ParticleSystem _hitEffect;
    [SerializeField] private bool _addTeamColorToEffects = true;
    [SerializeField] private bool _effectRotatesToHitDirection = true;
    [SerializeField, Range(0f, 1f)] private float _rotationToHitMultiplier = 0.2f;
    [SerializeField] private bool _effectInheritsProjectileDirection = false;
    
    private TeamData _currentTeam;
    private ParticleSystem _hitEffect;
    private ParticleSystem _splashEffect;
    private float _damage = 0f;
    private float _projectileSpeed = 1f;
    private float _projectileLifetime = 1f;
    private int _penetrationStrength = 0;        // Number of times the projectile can pierce through a target.
    private float _splashRadius = 0f;       // Splash damage.
    private LayerMask _splashMask;
    private float _elapsedLifetime = 0f;
    private int _penetrationCounter = 0;

    public float Damage => _damage;
    public TeamData CurrentTeam => _currentTeam;

    /*
    public void InitializeProjectile(TeamData newTeam, float newDamage, float newSpeed, float newLifetime, int newPenetrationStrength, float newSplashRadius, LayerMask newSplashMask)
    {
        _currentTeam = newTeam;
        _damage = newDamage;
        _projectileSpeed = newSpeed;
        _projectileLifetime = newLifetime;
        _penetrationStrength = newPenetrationStrength;
        _splashRadius = newSplashRadius;
        _splashMask = newSplashMask;
    }*/

    public void InitializeProjectile(TeamData newTeam, WeaponData newWeaponData)
    {
        _currentTeam = newTeam;
        _hitEffect = newWeaponData.HitEffect;
        _splashEffect = newWeaponData.SplashEffect;
        _damage = newWeaponData.Damage;
        _projectileSpeed = newWeaponData.ProjectileSpeed;
        _projectileLifetime = newWeaponData.ProjectileLifetime;
        _penetrationStrength = newWeaponData.PenetrationStrength;
        _splashRadius = newWeaponData.SplashRadius;
        _splashMask = newWeaponData.SplashMask;
    }

    private void Start()
    {
        _characterArtController.Initialize(_currentTeam);   // this is at start instead of awake because the projectile needs to be initialized first to get the team, before the artcontroller gets intialized.
    }

    private void Update()
    {
        if (_elapsedLifetime >= _projectileLifetime)
        {
            SpawnEffect(_hitEffect);
            DoSplashDamage();
            DestroyProjectile();
        }
        _elapsedLifetime += Time.deltaTime;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        _rb.MovePosition(transform.position + (transform.up * _projectileSpeed * Time.fixedDeltaTime));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out CharacterAgent collidingAgent))
        {
            if (collidingAgent.CurrentTeam != _currentTeam)
            {
                SpawnEffect(_hitEffect, collision);
                DoSplashDamage(collidingAgent);
                if (_penetrationCounter >= _penetrationStrength) DestroyProjectile();
                else _penetrationCounter++;
            }
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

    // This is so that the primary damaged agent doesn't receive damage twice.
    private void DoSplashDamage(CharacterAgent agentToIgnore)
    {
        if (_splashRadius == 0f) return;
        List<Collider2D> agentsWithinRange = Physics2D.OverlapCircleAll(transform.position, _splashRadius, _splashMask).ToList();
        for (int i = 0; i < agentsWithinRange.Count; i++)
        {
            if (agentsWithinRange[i].TryGetComponent(out CharacterAgent agent)
                && agent.CurrentTeam != CurrentTeam
                && agent != agentToIgnore)
            {
                agent.DamageCharacter(_damage);
                SpawnEffect(_splashEffect);
            }
        }
    }

    private void SpawnEffect(ParticleSystem effectToSpawn)
    {
        if (!effectToSpawn) return;

        Quaternion effectRotation = Quaternion.identity;
        if (_effectInheritsProjectileDirection)
        {
            effectRotation = transform.rotation;
        }

        ParticleSystem effect = Instantiate(effectToSpawn, transform.position, effectRotation);

        if (_addTeamColorToEffects)
        {
            ParticleSystem.MainModule effectMainModule = effect.main;
            effectMainModule.startColor = _currentTeam.TeamColor;
        }
    }

    private void SpawnEffect(ParticleSystem effectToSpawn, Collision2D collision)
    {
        if (!effectToSpawn) return;
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

        ParticleSystem effect = Instantiate(effectToSpawn, collision.GetContact(0).point, effectRotation);
        if (_addTeamColorToEffects)
        {
            ParticleSystem.MainModule effectMainModule = effect.main;
            effectMainModule.startColor = _currentTeam.TeamColor;
        }
    }
    /*
    private void SpawnHitEffect()
    {
        if (!_enableHitEffect || !_hitEffect) return;
        Quaternion effectRotation = Quaternion.identity;
        effectRotation = transform.rotation;
        ParticleSystem effect = Instantiate(_hitEffect, transform.position, effectRotation);
        if (_addTeamColorToEffects)
        {
            ParticleSystem.MainModule effectMainModule = effect.main;
            effectMainModule.startColor = _currentTeam.TeamColor;
        }
    }*/

    private void DestroyProjectile()
    {
        //gameObject.SetActive(false);
        Destroy(gameObject);
    }

}
