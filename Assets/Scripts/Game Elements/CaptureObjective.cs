using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureObjective : MonoBehaviour
{
    // This capture objective only tracks 1 team's progress at a time, using a Slider.
    [Header("References")] //
    [SerializeField] private ResourceBar _progressBar;
    [SerializeField] private Image _progressBarTeamColor;
    [SerializeField] private TeamData _initialTeamOwner;

    private List<CharacterAgent> _occupyingAgents = new List<CharacterAgent>();
    private TeamData _currentTeam;
    private float _currentProgress = 0f;

    private void EvaluateOccupyingAgents()
    {
        // Progress based on agents in capture area.
        TeamData occupyingTeam = null;
        foreach (CharacterAgent agent in _occupyingAgents)
        {
            if (!occupyingTeam)
            {
                occupyingTeam = agent.CurrentTeam;
            }
            if (agent.CurrentTeam != occupyingTeam)
            {
                //
            }
        }
    }

    private void UpdateProgress()
    {
        //  increment based on fixed speed
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Register new enemies here.
        CharacterAgent agent = collision.GetComponent<CharacterAgent>();
        if (agent && !_occupyingAgents.Contains(agent)) _occupyingAgents.Add(agent);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Remove enemies present in list when it exits the capture area.
        CharacterAgent agentToEvaluate = collision.GetComponent<CharacterAgent>();
        if (!agentToEvaluate) return;
        if (_occupyingAgents.Contains(agentToEvaluate))
        {
            _occupyingAgents.Remove(agentToEvaluate);
        }
    }

    private void OnDrawGizmos()
    {
        // Debug radius.
        Gizmos.color = Color.green;
    }
}