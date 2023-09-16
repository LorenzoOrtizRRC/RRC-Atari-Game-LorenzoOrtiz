using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class WeaponInstance : MonoBehaviour
{
    [SerializeField] protected WeaponData _weaponData;
    [SerializeField] protected List<Transform> _projectileSpawnPoints = new List<Transform>();  // positions used to position spawned projectiles

    public float MinimumRange => _weaponData.MinimumWeaponRange;
    public float MaximumRange => _weaponData.MaximumWeaponRange;

    private Team _currentTeam = Team.cat;
    private CharacterAgent _enemyAgent = null; // current target enemy
    private float _cooldownTime = 0f;    // from (1 / rate of fire) to 0. When <= 0, weapon is ready to fire.
    private bool _canFire = true;
    private bool _isFiring = false;

    private void Update()
    {
        if (_enemyAgent) RotateWeapon();
    }

    public void InitializeWeapon(Team currentTeam) => _currentTeam = currentTeam;

    // activation conditions for weapon
    public void UseWeapon(CharacterAgent enemyAgent)
    {
        //  rotate weapon towards target
        float angleDifference = Vector2.SignedAngle(transform.up, (enemyAgent.transform.position - transform.position));

        //  fire weapon if: rotation is correct, is off cooldown, and is within minimum and maximum range
        if (Mathf.Abs(angleDifference) > 1f && Time.time >= _cooldownTime)
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

    public void RotateWeapon()
    {
        float angleDifference = Vector2.SignedAngle(transform.up, (_enemyAgent.transform.position - transform.position));
        float direction = angleDifference > 0f ? 1f : -1f;
        float absClampValue = Mathf.Abs(angleDifference);
        transform.Rotate(new Vector3(0f, 0f, 1 * Mathf.Clamp(direction * 180f * Time.deltaTime, -absClampValue, absClampValue)));
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

    public void SetNewTarget(CharacterAgent enemyAgent) => _enemyAgent = enemyAgent;
}
