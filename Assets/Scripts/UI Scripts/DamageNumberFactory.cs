using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumberFactory : MonoBehaviour
{
    [SerializeField] private GameObject _damageNumberPrefab;
    [SerializeField] private float _spawnAngleDifference = 10f;
    // this factory creates damage numbers based on events tied to created CharacterAgent(s)
    public void CreateDamageNumber(Collision2D damageSource, float mitigatedDamage)
    {
        GameObject damageNumberObject = GenericObjectPooler.CurrentInstance.GetGameObjectFromPool(_damageNumberPrefab);
        DamageNumber damageNumberComponent = damageNumberObject.GetComponent<DamageNumber>();
        if (!damageNumberComponent) return;
        Vector2 direction = Quaternion.AngleAxis(_spawnAngleDifference, Vector3.forward) * damageSource.transform.forward;
        damageNumberComponent?.InitializeDamageNumber(mitigatedDamage, damageSource.transform.position, direction);
    }

    // agents created from
    public void GetCreatedAgent(CharacterAgent createdAgent)
    {
        //
    }
}
