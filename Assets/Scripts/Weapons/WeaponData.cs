using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : ScriptableObject
{
    [SerializeField] private GameObject _projectileObject;
    [SerializeField] private float _damage;
    [SerializeField] private float _rateOfFire;     // Projectiles per second. Used to calculate weapon cooldown between bullets. Formula for cooldown: 1 second / _rateOfFire.
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _projectileLifetime;     // OProjectile lifetime duration in seconds.

    public GameObject ProjectileObject => _projectileObject;
    public float Damage => _damage;
    public float RateOfFire => _rateOfFire;
    public float ProjectileSpeed => _projectileSpeed;
    public float ProjectileLifetime => _projectileLifetime;
}
