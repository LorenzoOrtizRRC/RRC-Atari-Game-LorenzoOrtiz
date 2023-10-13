using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [SerializeField] private GameObject _projectileObject;
    [SerializeField] private float _damage = 1f;
    [SerializeField] private float _rateOfFire = 1f;     // Projectiles per second. Used to calculate weapon cooldown between bullets. Formula for rate of fire: 1 second / bullets per second.
    [SerializeField] private float _cooldownAfterFiring = 0f;       // Starts after weapon fires a projectile, where weapon will cease to function until cooldown ends (including firing and rotations).
    [SerializeField] private float _rotationSpeed = 180f;
    [SerializeField, Range(0f, 180f)] private float _maxAngleToShoot = 1f;      // Weapon will fire projectiles only when the angle from its target direction is below this number.
    [SerializeField] private float _minimumWeaponRange = 0f;
    [SerializeField] private float _maximumWeaponRange = 5f;
    [SerializeField] private float _weaponAngleSpread = 0f; // spread for projectiles in degrees
    [SerializeField] private float _projectileSpeed = 1f;
    [SerializeField] private float _projectileLifetime = 1f;     // Projectile lifetime duration in seconds.

    public GameObject ProjectileObject => _projectileObject;
    public float Damage => _damage;
    public float RateOfFire => _rateOfFire;
    public float RotationSpeed => _rotationSpeed;
    public float MinimumWeaponRange => _minimumWeaponRange;
    public float MaximumWeaponRange => _maximumWeaponRange;
    public float WeaponAngleSpread => _weaponAngleSpread;
    public float ProjectileSpeed => _projectileSpeed;
    public float ProjectileLifetime => _projectileLifetime;
}
