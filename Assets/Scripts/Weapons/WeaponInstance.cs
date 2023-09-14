using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponInstance : MonoBehaviour
{
    [SerializeField] protected WeaponData _weaponData;
    [SerializeField] protected List<Transform> _projectileSpawnPoints = new List<Transform>();     // positions used to position spawned projectiles
    protected abstract void UseWeapon();
    protected virtual void FireProjectile()
    {
        //Instantiate()
    }
}
