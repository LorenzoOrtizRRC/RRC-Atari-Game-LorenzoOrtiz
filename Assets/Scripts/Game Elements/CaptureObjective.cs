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
    [SerializeField] private SpriteRenderer _objectiveColors;
    [Header("Objective Variables")]
    [SerializeField] private TeamData _neutralTeamOwner;    // When unoccupied, or previous team owner is removed and capturing process by a new team starts, set team owner to this.
    [SerializeField] private TeamData _initialTeamOwner;
    [SerializeField] private float _captureSpeed = 0.5f;       // % capture speed per second.
    [SerializeField] private bool _hideProgressBarWhenFull = true;

    private List<CharacterAgent> _occupyingAgents = new List<CharacterAgent>();
    private TeamData _currentOccupyingTeam;     // The team that currently occupies this objective. Used for when a team interrupts another team's capture of a neutral capture objective.
    private TeamData _ownerTeam;        // The team that currently owns this objective.
    private float _currentProgress = 0f;        // Max 1 (100%) progress.

    private void Awake()
    {
        // Initialize ownership.
        SetNewObjectiveOwner(_initialTeamOwner);
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
        //UpdateProgressOwner(occupyingTeam);

        //if (_hideProgressBarWhenFull && _progressBar.gameObject.activeInHierarchy && _currentProgress / 1f == 1f) _progressBar.gameObject.SetActive(false);
        //else if (!_progressBar.gameObject.activeInHierarchy && _currentProgress / 1f < 1f) _progressBar.gameObject.SetActive(true);
    }

    private void UpdateProgress(TeamData occupyingTeam)
    {
        /*
        if (occupyingTeam == _ownerTeam && _currentProgress == 1f) return;
        else if (_currentProgress == 0f)        // If progress is 0, switch owners
        {
            if (_ownerTeam != occupyingTeam && _ownerTeam != _neutralTeamOwner)
            {
                //_currentTeamOccupants = occupyingTeam;
                // Change bar color to occupying team. Change owner to neutral.
                UpdateProgressOwner(occupyingTeam);
                UpdateOwnerTeam(_neutralTeamOwner);
            }
            else return;
        }

        // Increment or decrement current progress based on current team ownership vs. currently occupying team.
        if (_ownerTeam != occupyingTeam)
        {
            _currentProgress = Mathf.Clamp01(_currentProgress - (_captureSpeed * Time.deltaTime));
        }
        else
        {
            _currentProgress = Mathf.Clamp01(_currentProgress + (_captureSpeed * Time.deltaTime));
        }
        _progressBar.UpdateSliderValue(_currentProgress / 1f);
        */

        /*
        if the occupants own objective OR if its neutral;
            if its 100%
                if its not theirs, make it theirs. Otherwise do nothing.
            if it's less than 100, increment
        if they dont (the enemy owns it);
            if it's 0, and the owner isnt neutral, make it neutral
            if it's the enemy's, decrement
           
        */
        //if (_ownerTeam == _neutralTeamOwner && )
        /*
        if (_ownerTeam == _neutralTeamOwner)
        {
            if (_currentProgress == 1f) SetNewObjectiveOwner(occupyingTeam);
            else _currentProgress = Mathf.Clamp01(_currentProgress + (_captureSpeed * Time.deltaTime));
        }
        else if (_ownerTeam == occupyingTeam)
        {
            if (_currentProgress < 1f) _currentProgress = Mathf.Clamp01(_currentProgress + (_captureSpeed * Time.deltaTime));
        }
        else
        {
            if (_currentProgress == 0f)
            {
                UpdateProgressBarColor(occupyingTeam);
                SetNewObjectiveOwner(_neutralTeamOwner);
            }
            else _currentProgress = Mathf.Clamp01(_currentProgress + (_captureSpeed * Time.deltaTime));
        }
        */

        if (_ownerTeam == occupyingTeam)
        {
            if (_currentProgress < 1f)
            {
                _currentProgress = Mathf.Clamp01(_currentProgress + (_captureSpeed * Time.deltaTime));
                _progressBar.UpdateSliderValue(_currentProgress);
                return;
            }
        }
        else if (_ownerTeam != _neutralTeamOwner)
        {
            if (_currentProgress == 0f)
            {
                _currentOccupyingTeam = occupyingTeam;
                UpdateProgressBarColor(occupyingTeam);
                SetNewObjectiveOwner(_neutralTeamOwner);
            }
            else
            {
                _currentProgress = Mathf.Clamp01(_currentProgress - (_captureSpeed * Time.deltaTime));
                _progressBar.UpdateSliderValue(_currentProgress);
            }
        }
        else
        {
            // owner team is neutral
            if (_currentProgress == 1f) SetNewObjectiveOwner(occupyingTeam);
            else
            {
                if (occupyingTeam != _currentOccupyingTeam)
                {
                    if (_currentProgress == 0f)
                    {
                        _currentOccupyingTeam = occupyingTeam;
                        UpdateProgressBarColor(occupyingTeam);
                    }
                    else
                    {
                        _currentProgress = Mathf.Clamp01(_currentProgress - (_captureSpeed * Time.deltaTime));
                        _progressBar.UpdateSliderValue(_currentProgress);
                    }
                }
                else
                {
                    _currentProgress = Mathf.Clamp01(_currentProgress + (_captureSpeed * Time.deltaTime));
                    _progressBar.UpdateSliderValue(_currentProgress);
                }
            }
        }
    }

    // This method changes the color of the progress bar to indicate which team is capturing the objective (from neutral & 0 progress)
    private void UpdateProgressBarColor(TeamData newTeam)
    {
        _progressBarTeamColor.color = newTeam.TeamColor;
    }

    // This method changes the colors of the objective itself to indicate which team owns the objective.
    private void SetNewObjectiveOwner(TeamData newTeam)
    {
        _ownerTeam = newTeam;
        _objectiveColors.color = newTeam.TeamColor;
        // Change objective colors.
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