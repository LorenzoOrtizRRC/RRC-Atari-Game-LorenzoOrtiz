using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponInstance : MonoBehaviour
{
    [SerializeField] protected WeaponData _weaponData;
    [SerializeField] protected List<Transform> _projectileSpawnPoints = new List<Transform>();  // positions used to position spawned projectiles

    private float _cooldownTime = 0f;    // from (1 / rate of fire) to 0. When <= 0, weapon is ready to fire.
    private bool _canFire = true;
    private bool _isFiring = false;

    public void UseWeapon(Team weaponOwnerTeam)
    {
        if (Time.time >= _cooldownTime)
        {
            FireWeapon(weaponOwnerTeam);
            _cooldownTime = Time.time + _weaponData.RateOfFire;
        }
    }

    public virtual void FireWeapon(Team weaponOwnerTeam)
    {
        foreach (Transform spawnPoint in _projectileSpawnPoints)
        {
            ProjectileInstance spawnedProjectile = SpawnProjectile(spawnPoint);
            InitializeProjectile(spawnedProjectile, weaponOwnerTeam);
        }
    }

    private ProjectileInstance SpawnProjectile(Transform spawnPosition)
    {
        ProjectileInstance spawnedProjectile = Instantiate(_weaponData.ProjectileObject, spawnPosition.position, spawnPosition.rotation)
            .GetComponent<ProjectileInstance>();
        return spawnedProjectile;
    }

    private void InitializeProjectile(ProjectileInstance spawnedProjectile, Team projectileTeam)
    {
        spawnedProjectile.InitializeProjectile(_weaponData.Damage, _weaponData.ProjectileSpeed, _weaponData.ProjectileLifetime, projectileTeam);
    }
}
