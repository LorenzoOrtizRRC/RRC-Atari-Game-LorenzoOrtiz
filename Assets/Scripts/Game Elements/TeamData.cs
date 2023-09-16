using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team { cat = 0, dog = 1 }

[CreateAssetMenu(fileName = "New Team Data", menuName ="Game/Team Data")]
public class TeamData : ScriptableObject
{
    [SerializeField] private Team _selectedTeam;
    [SerializeField] private string _teamName;
    [SerializeField] private Color _teamColor;
}