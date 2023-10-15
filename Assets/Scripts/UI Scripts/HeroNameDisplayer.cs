using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeroNameDisplayer : MonoBehaviour
{
    [SerializeField] private CharacterAgent _agent;
    [SerializeField] private TextMeshProUGUI _textDisplay;

    private void Start()
    {
        _textDisplay.text = _agent.AgentName;
        _textDisplay.color = _agent.CurrentTeam.TeamColor;
    }
}
