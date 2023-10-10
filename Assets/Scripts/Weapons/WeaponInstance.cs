using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponInstance : MonoBehaviour
{
    [SerializeField] protected WeaponData _weaponData;
    [SerializeField] protected List<Transform> _projectileSpawnPoints = new List<Transform>();  // positions used to position spawned projectiles
    [SerializeField, Header("Debugging")] private bool _visualizeWeaponSpread = true; 

    public float MinimumRange => _weaponData.MinimumWeaponRange;
    public float MaximumRange => _weaponData.MaximumWeaponRange;

    private TeamData _currentTeam;
    private float _cooldownTime = 0f;    // from (1 / rate of fire) to 0. When <= 0, weapon is ready to fire.

    public void InitializeWeapon(TeamData currentTeam) => _currentTeam = currentTeam;

    // activation conditions for weapon
    public void UseWeaponAuto(Vector2 targetPoint)
    {
        // targetPoint can be the direction the agent is moving, or the enemy's position relative to weapon
        Vector2 direction = targetPoint - (Vector2)transform.position;
        //  rotate weapon towards target
        float angleDifference = Vector2.SignedAngle(transform.up, direction);
        //  fire weapon if: rotation is correct, is off cooldown, and is within minimum and maximum range
        if (Mathf.Abs(angleDifference) <= 1f && Time.time >= _cooldownTime)
        {
            FireWeapon();
            _cooldownTime = Time.time + _weaponData.RateOfFire;
        }
    }

    // logic for spawning projectiles
    public virtual void FireWeapon()
    {
        foreach (Transform spawnPoint in _projectileSpawnPoints)
        {
            ProjectileInstance spawnedProjectile = SpawnProjectile(spawnPoint);
            InitializeProjectile(spawnedProjectile);
        }
    }

    public void RotateWeapon(Vector2 targetPoint)
    {
        Vector2 direction = targetPoint - (Vector2)transform.position;
        float angleDifference = Vector2.SignedAngle(transform.up, direction);
        float rotationDirection = angleDifference > 0f ? 1f : -1f;
        float absClampValue = Mathf.Abs(angleDifference);
        transform.Rotate(new Vector3(0f, 0f, 1 * Mathf.Clamp(rotationDirection * _weaponData.RotationSpeed * Time.fixedDeltaTime, -absClampValue, absClampValue)));
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
        spawnedProjectile.InitializeProjectile(_weaponData.Damage, _weaponData.ProjectileSpeed, _weaponData.ProjectileLifetime, _currentTeam);
    }

    private void OnDrawGizmos()
    {
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
