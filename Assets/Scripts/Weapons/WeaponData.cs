using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [SerializeField] private GameObject _projectileObject;
    [SerializeField] private float _damage;
    [SerializeField] private float _rateOfFire;     // Projectiles per second. Used to calculate weapon cooldown between bullets. Formula for rate of fire: 1 second / bullets per second.
    [SerializeField] private float _minimumWeaponRange = 0f;
    [SerializeField] private float _maximumWeaponRange = 5f;
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _projectileLifetime;     // Projectile lifetime duration in seconds.

    public GameObject ProjectileObject => _projectileObject;
    public float Damage => _damage;
    public float RateOfFire => _rateOfFire;
    public float ProjectileSpeed => _projectileSpeed;
    public float ProjectileLifetime => _projectileLifetime;
}
