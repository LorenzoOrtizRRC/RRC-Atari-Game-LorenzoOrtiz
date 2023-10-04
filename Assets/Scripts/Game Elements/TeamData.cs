using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Team Data", menuName ="Game/Team Data")]
public class TeamData : ScriptableObject
{
    [SerializeField] private string _teamName;
    [SerializeField] private Color _teamColor;

    public string TeamName => _teamName;
    public Color TeamColor => _teamColor;
}