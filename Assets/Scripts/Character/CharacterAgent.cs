using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class CharacterAgent : MonoBehaviour
{
    // This class is in charge of managing the instance of the unit in the level, including tracking its live stats.

    [SerializeField] private CharacterData _stats;
    private float _currentHealth;

    public float MaxHealth => _stats.Health;
    public float Armor => _stats.Armor;
    public float Speed => _stats.Speed;
    public float CurrentHealth => _currentHealth;


    private void Start()
    {
        //
    }
}
