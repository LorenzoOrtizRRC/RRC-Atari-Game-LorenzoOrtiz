using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponInstance : MonoBehaviour
{
    [SerializeField] protected WeaponData _weaponData;
    [SerializeField] protected List<Transform> _projectileSpawnPoints = new List<Transform>();  // positions used to position spawned projectiles
    [Header("Weapon Settings")]
    [SerializeField] private bool _disableRotation = false;
    [Header("Effects")]
    [SerializeField] private bool _enableEffects = true;
    [SerializeField] private ParticleSystem _firingEffect;
    [SerializeField] private bool _addTeamColorsToEffects = true;
    [Header("Debugging")]
    [SerializeField] private bool _enableGizmos = false;
    [SerializeField] private bool _visualizeWeaponSpread = false; 

    public float MinimumRange => _weaponData.MinimumWeaponRange;
    public float MaximumRange => _weaponData.MaximumWeaponRange;

    private TeamData _currentTeam;
    private float _firingTimer = 0f;    // from (1 / rate of fire) to 0. When <= 0, weapon is ready to fire.
    private float _delayAfterFiringTimer = 0f;

    public void InitializeWeapon(TeamData currentTeam) => _currentTeam = currentTeam;

    // activation conditions for weapon
    public void UseWeaponAuto(Vector2 targetPoint)
    {
        // targetPoint can be the direction the agent is moving, or the enemy's position relative to weapon
        Vector2 direction = targetPoint - (Vector2)transform.position;
        //  rotate weapon towards target
        float angleDifference = Vector2.Angle(transform.up, direction);
        //  fire weapon if: rotation is correct, is off cooldown, and is within minimum and maximum range
        if (Time.time > _delayAfterFiringTimer)
        {
            if (angleDifference <= _weaponData.MaxAngleToShoot && Time.time >= _firingTimer)
            {
                FireWeapon();
                _firingTimer = Time.time + _weaponData.RateOfFire;
                _delayAfterFiringTimer = Time.time + _weaponData.DelayAfterFiring;
            }
        }
    }

    // logic for spawning projectiles
    public virtual void FireWeapon()
    {
        foreach (Transform spawnPoint in _projectileSpawnPoints)
        {
            ProjectileInstance spawnedProjectile = SpawnProjectile(spawnPoint);
            InitializeProjectile(spawnedProjectile);
            if (_enableEffects && _firingEffect)
            {
                ParticleSystem.MainModule effectMainModule = Instantiate(_firingEffect, spawnPoint.position, spawnPoint.rotation).main;
                if (_addTeamColorsToEffects) effectMainModule.startColor = _currentTeam.TeamColor;
            }
        }
    }

    public void RotateWeapon(Vector2 targetPoint)
    {
        if (_disableRotation) return;
        Vector2 direction = targetPoint - (Vector2)transform.position;
        float angleDifference = Vector2.SignedAngle(transform.up, direction);
        //float rotationDirection = angleDifference > 0f ? 1f : -1f;
        //float absClampValue = Mathf.Abs(angleDifference);
        //transform.Rotate(new Vector3(0f, 0f, 1 * Mathf.Clamp(rotationDirection * _weaponData.RotationSpeed * Time.fixedDeltaTime, -absClampValue, absClampValue)));

        float angleDirection = 0f;
        //if (avoidDirection) angleDirection = Mathf.Sign(angleDifference) * 180f;
        // Mathf.MoveTowardsAngle made me cry. I'm never using it here ever. EVER.
        float stepAngle = Mathf.MoveTowards(angleDifference, angleDirection, _weaponData.RotationSpeed * Time.fixedDeltaTime);
        transform.rotation *= Quaternion.AngleAxis(angleDifference - stepAngle, Vector3.forward);
    }

    public void RotateWeaponInstant(Vector2 targetPoint)
    {
        if (_disableRotation) return;
        Vector2 direction = targetPoint - (Vector2)transform.position;
        float angleDifference = Vector2.SignedAngle(transform.up, direction);
        transform.rotation *= Quaternion.AngleAxis(angleDifference, Vector3.forward);
    }

    private ProjectileInstance SpawnProjectile(Transform spawnPosition)
    {
        float spreadAngle = _weaponData.WeaponAngleSpread / 2f;
        Quaternion rotationWithSpread = spawnPosition.rotation * Quaternion.AngleAxis(Random.Range(-spreadAngle, spreadAngle), Vector3.forward);
        ProjectileInstance spawnedProjectile = Instantiate(_weaponData.ProjectileObject, spawnPosition.position, rotationWithSpread)
            .GetComponent<ProjectileInstance>();
        return spawnedProjectile;
    }

    private void InitializeProjectile(ProjectileInstance spawnedProjectile)
    {
        /*spawnedProjectile.InitializeProjectile(_currentTeam, _weaponData.Damage, _weaponData.ProjectileSpeed,
            _weaponData.ProjectileLifetime, _weaponData.PenetrationStrength, _weaponData.SplashRadius);*/
        spawnedProjectile.InitializeProjectile(_currentTeam, _weaponData);
    }

    private void OnDrawGizmos()
    {
        if (!_enableGizmos) return;
        // visualize weapon spread
        if (_visualizeWeaponSpread && _weaponData.WeaponAngleSpread != 0f)
        {
            Gizmos.color = Color.yellow;
            float spreadAngle = _weaponData.WeaponAngleSpread / 2f;
            Quaternion minRotation = _projectileSpawnPoints[0].localRotation * Quaternion.AngleAxis(-spreadAngle, Vector3.forward);
            Quaternion maxRotation = _projectileSpawnPoints[0].localRotation * Quaternion.AngleAxis(spreadAngle, Vector3.forward);
            foreach (Transform spawnPoint in _projectileSpawnPoints)
            {
                // Subtract distance between weapon position and spawn position to accurately visualize weapon range
                float lineLength = _weaponData.MaximumWeaponRange - (spawnPoint.transform.position - transform.position).magnitude;
                // for some reason only (Quaternion * Vector) is allowed, not (Vector * Quaternion)
                Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + ((minRotation * spawnPoint.up) * lineLength));
                Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + ((maxRotation * spawnPoint.up) * lineLength));
            }
        }
    }
}
