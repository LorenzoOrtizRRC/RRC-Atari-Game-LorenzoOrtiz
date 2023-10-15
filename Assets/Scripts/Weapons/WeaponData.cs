using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("References")]
    [SerializeField] private GameObject _projectileObject;
    [SerializeField] private ParticleSystem _hitEffect;
    [SerializeField] private ParticleSystem _splashEffect;
    [Header("Weapon Stats")]
    [SerializeField, Min(0f)] private float _damage = 1f;
    [SerializeField, Min(0f)] private float _rateOfFire = 1f;     // Projectiles per second. Used to calculate weapon cooldown between bullets. Formula for rate of fire: 1 second / bullets per second.
    [SerializeField, Min(0f)] private int _penetrationStrength = 0;
    [SerializeField, Min(0f)] private float _splashRadius = 0f;      // AoE damage. A value of 0 deactivates it.
    [SerializeField] private LayerMask _splashMask;
    [SerializeField] private float _delayAfterFiring = 0f;       // Starts after weapon fires a projectile, where weapon will cease to function until cooldown ends (including firing and rotations).
    [SerializeField, Min(0f)] private float _rotationSpeed = 180f;
    [SerializeField, Range(0f, 180f)] private float _maxAngleToShoot = 1f;      // Weapon will fire projectiles only when the angle from its target direction is below this number.
    [SerializeField, Min(0f)] private float _minimumWeaponRange = 0f;
    [SerializeField, Min(0f)] private float _maximumWeaponRange = 5f;
    [SerializeField, Min(0f)] private float _weaponAngleSpread = 0f; // spread for projectiles in degrees
    [SerializeField] private float _projectileSpeed = 1f;
    [SerializeField, Min(0f)] private float _projectileLifetime = 1f;     // Projectile lifetime duration in seconds.

    public GameObject ProjectileObject => _projectileObject;
    public ParticleSystem HitEffect => _hitEffect;
    public ParticleSystem SplashEffect => _splashEffect;
    public float Damage => _damage;
    public float RateOfFire => _rateOfFire;
    public int PenetrationStrength => _penetrationStrength;
    public float SplashRadius => _splashRadius;
    public LayerMask SplashMask => _splashMask;
    public float DelayAfterFiring => _delayAfterFiring;
    public float RotationSpeed => _rotationSpeed;
    public float MaxAngleToShoot => _maxAngleToShoot;
    public float MinimumWeaponRange => _minimumWeaponRange;
    public float MaximumWeaponRange => _maximumWeaponRange;
    public float WeaponAngleSpread => _weaponAngleSpread;
    public float ProjectileSpeed => _projectileSpeed;
    public float ProjectileLifetime => _projectileLifetime;
}
