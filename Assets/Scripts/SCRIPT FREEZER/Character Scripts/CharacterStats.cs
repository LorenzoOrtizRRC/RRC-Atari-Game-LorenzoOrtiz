using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Stats", menuName = "Character/Character Stats")]
public class CharacterStats : ScriptableObject
{
    // CharacterData is a container storing basic stats.
    public float Health { get { return _health; } }
    public float Energy { get { return _energy; } }
    public int Armor { get { return _armor; } }

    [SerializeField] private float _health = 1f;
    [SerializeField] private float _energy = 0f;
    [SerializeField] private int _armor = 0;
}
