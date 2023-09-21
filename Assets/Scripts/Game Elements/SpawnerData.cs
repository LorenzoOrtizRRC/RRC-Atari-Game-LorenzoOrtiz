using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spawner Data", menuName = "Game Elements/Spawner Data")]
public class SpawnerData : ScriptableObject
{
    [SerializeField] private GameObject _minionPrefab;
    [SerializeField] private int _numberOfMinions;
}
