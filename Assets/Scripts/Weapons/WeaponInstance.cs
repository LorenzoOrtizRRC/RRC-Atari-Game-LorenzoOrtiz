using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponInstance : MonoBehaviour
{
    [SerializeField] protected WeaponData _weaponData;
    [SerializeField] protected List<Transform> _projectileSpawnPoints = new List<Transform>();  // positions used to position spawned projectiles

    private Team _currentTeam = Team.cat;
    private float _cooldownTime = 0f;    // from (1 / rate of fire) to 0. When <= 0, weapon is ready to fire.
    private bool _canFire = true;
    private bool _isFiring = false;

    public void InitializeWeapon(Team currentTeam)
    {
        _currentTeam = currentTeam;
    }

    // activation conditions for weapon
    public void UseWeapon(CharacterAgent enemyAgent)
    {
        //  rotate weapon towards target
        float angleDifference = Vector2.SignedAngle(transform.up, (enemyAgent.transform.position - transform.position));
        if (Mathf.Abs(angleDifference) > 1f)
        {
            float direction = angleDifference > 0f ? 1f : -1f;
            transform.Rotate(new Vector3(0f, 0f, 1 * direction * 180f * Time.deltaTime));
        }
        else if (Time.time >= _cooldownTime)
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

    private ProjectileInstance SpawnProjectile(Transform spawnPosition)
    {
        ProjectileInstance spawnedProjectile = Instantiate(_weaponData.ProjectileObject, spawnPosition.position, spawnPosition.rotation)
            .GetComponent<ProjectileInstance>();
        return spawnedProjectile;
    }

    private void InitializeProjectile(ProjectileInstance spawnedProjectile)
    {
        spawnedProjectile.InitializeProjectile(_weaponData.Damage, _weaponData.ProjectileSpeed, _weaponData.ProjectileLifetime, _currentTeam);
    }
}
