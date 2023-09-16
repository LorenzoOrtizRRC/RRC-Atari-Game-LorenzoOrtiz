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

    public Team SelectedTeam => _selectedTeam;
    public string TeamName => _teamName;
    public Color TeamColor => _teamColor;
}