using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureObjective : MonoBehaviour
{
    // This capture objective only tracks 1 team's progress at a time, using a Slider.
    [Header("References")]
    [SerializeField] private Collider2D _objectiveCollider;
    [SerializeField] private ResourceBar _progressBar;
    [SerializeField] private Image _progressBarTeamColor;
    [SerializeField] private TeamData _initialTeamOwner;
    [Header("Objective Variables")]
    [SerializeField] private float _captureSpeed = 0.5f;       // % capture speed per second.
    [SerializeField] private bool _hideProgressBarWhenFull = true;

    private List<CharacterAgent> _occupyingAgents = new List<CharacterAgent>();
    private TeamData _currentTeamOccupants;     // The team that currently occupies this objective.
    private TeamData _teamOwner;        // The team that currently owns this objective.
    private float _currentProgress = 0f;        // Max 1 (100%) progress.

    private void Awake()
    {
        _currentProgress = 1f;      // Set to max.
        _progressBar.UpdateSliderValue(_currentProgress / 1f);
    }

    private void Update()
    {
        if (_occupyingAgents.Count > 0) EvaluateOccupyingAgents();
    }

    private void EvaluateOccupyingAgents()
    {
        // Check which teams' agents are in the capture area.
        TeamData occupyingTeam = null;
        foreach (CharacterAgent agent in _occupyingAgents)
        {
            if (!occupyingTeam)
            {
                occupyingTeam = agent.CurrentTeam;
            }
            // If there are agents of multiple teams, do not update progress.
            if (agent.CurrentTeam != occupyingTeam) return;
        }
        UpdateProgress(occupyingTeam);

        if (_hideProgressBarWhenFull && _progressBar.gameObject.activeInHierarchy && _currentProgress / 1f == 1f) _progressBar.gameObject.SetActive(false);
        else _progressBar.gameObject.SetActive(true);
    }

    private void UpdateProgress(TeamData occupyingTeam)
    {
        if (_currentProgress == 0f)
        {
            if (_currentTeamOccupants != occupyingTeam)
            {
                _currentTeamOccupants = occupyingTeam;
                UpdateTeamColor(occupyingTeam);
            }
            else return;
        }
        // Increment or decrement current progress based on current team ownership vs. currently occupying team.
        if (_currentTeamOccupants != occupyingTeam)
        {
            _currentProgress = Mathf.Clamp01(_currentProgress - (_captureSpeed * Time.deltaTime));
        }
        else if (_currentProgress == 1f)
        {
            //
        }
        else
        {
            _currentProgress = Mathf.Clamp01(_currentProgress + (_captureSpeed * Time.deltaTime));
        }
        _progressBar.UpdateSliderValue(_currentProgress / 1f);
    }

    private void UpdateTeamColor(TeamData occupyingTeam)
    {
        _progressBarTeamColor.color = occupyingTeam.TeamColor;
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
        // Debug objective area.
        Gizmos.color = Color.green;
        if (_objectiveCollider is CircleCollider2D circleCollider)
        {
            Gizmos.DrawWireSphere(transform.position, circleCollider.radius);
        }
        else if (_objectiveCollider is BoxCollider2D boxCollider) Gizmos.DrawWireCube(transform.position, boxCollider.size);
    }
}