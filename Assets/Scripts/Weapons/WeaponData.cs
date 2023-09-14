using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : ScriptableObject
{
    [SerializeField] private float _damage;
    [SerializeField] private float _rateOfFire;     // Projectiles per second. Used to calculate weapon cooldown between bullets. Formula for cooldown: 1 second / _rateOfFire.
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _projectileLifetime;     // OProjectile lifetime duration in seconds.
    [SerializeField] private GameObject _projectileObject;
}
