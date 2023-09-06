using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Data", menuName = "Character/Character Data")]
public class CharacterData : ScriptableObject
{
    // ScriptableObject list for Weapons
    // Destructible interface, Damage implementation

    // other stuff like active weapons, current hp, etc. will be handled by the active agent/unit
    // in the level that uses this data

    [SerializeField] private CharacterStats _characterStats;


}
