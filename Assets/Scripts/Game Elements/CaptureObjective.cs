using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CaptureObjective : MonoBehaviour
{
    public UnityEvent<TeamData> OnObjectiveCaptured;    // Fires when a new ownerTeam that isn't _neutralTeamOwner is assigned.
    public UnityEvent<TeamData> OnObjectiveLost;        // Fires when objective turns neutral.

    // This capture objective only tracks 1 team's progress at a time, using a Slider.
    [Header("References")]
    [SerializeField] private Collider2D _objectiveCollider;
    [SerializeField] private ResourceBar _progressBar;
    [SerializeField] private Image _progressBarTeamColor;
    [SerializeField] private SpriteRenderer[] _objectiveColors;
    [Header("Objective Variables")]
    [SerializeField] private TeamData _neutralTeamOwner;    // When unoccupied, or previous team owner is removed and capturing process by a new team starts, set team owner to this.
    [SerializeField] private TeamData _initialTeamOwner;
    [SerializeField] private float _captureSpeed = 0.5f;       // % capture speed per second.
    [SerializeField] private float _captureRegenDelay = 10f;        // Time in real seconds after being unoccupied to slowly increase/decrease capture progress. Set to 0f to disable.
    [SerializeField] private float _captureRegenSpeed = 0.5f;       // % capture regeneration speed (back to full or 0) per second when unoccupied after _captureRegenDelay seconds.
    [SerializeField] private bool _hideProgressBarWhenFull = true;
    [Header("Colors")]
    [SerializeField] private bool _preserveAlphaTransparency = true;


    private List<CharacterAgent> _occupyingAgents = new List<CharacterAgent>();
    private TeamData _currentOccupyingTeam;     // The team that currently occupies this objective. Used for when a team interrupts another team's capture of a neutral capture objective.
    private TeamData _ownerTeam;        // The team that currently owns this objective.
    private float _currentProgress = 0f;        // Max 1 (100%) progress.
    private float _captureRegenTimer = 0f;

    public TeamData NeutralTeamOwner => _neutralTeamOwner;
    public TeamData OwnerTeam => _ownerTeam;

    protected virtual void Awake()
    {
        // Initialize ownership.
        SetNewOwner(_initialTeamOwner);
        UpdateProgressBarColor(_initialTeamOwner);
        _currentProgress = _ownerTeam == _neutralTeamOwner ? 0f : 1f;
        _progressBar.UpdateSliderValue(_currentProgress / 1f);

        // Set progress bar visibility.
        if (_hideProgressBarWhenFull)
        {
            if (_progressBar.gameObject.activeInHierarchy && _currentProgress / 1f == 1f) _progressBar.gameObject.SetActive(false);     // Disable if full progress.
            else if (!_progressBar.gameObject.activeInHierarchy && _currentProgress / 1f < 1f) _progressBar.gameObject.SetActive(true);
            else if (_currentProgress == 0f && _ownerTeam == NeutralTeamOwner) _progressBar.gameObject.SetActive(false);    // Disable if neutral & 0 progress.
            //OnObjectiveCaptured?.Invoke(_ownerTeam);
        }
    }

    private void OnEnable()
    {
        _captureRegenTimer = Time.time + _captureRegenDelay;
    }

    private void Start()
    {
        OnObjectiveCaptured?.Invoke(_ownerTeam);
    }

    private void Update()
    {
        if (_occupyingAgents.Count > 0) EvaluateOccupyingAgents();
        else if (Time.time >= _captureRegenTimer) RegenerateProgress();
    }

    private void EvaluateOccupyingAgents()
    {
        // Capture area is occupied, so reset unoccupied regen timer.
        _captureRegenTimer = Time.time + _captureRegenDelay;

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

        // Toggle progress bar visibility based on progress
        if (_hideProgressBarWhenFull && _progressBar.gameObject.activeInHierarchy && _currentProgress / 1f == 1f) _progressBar.gameObject.SetActive(false);
        else if (!_progressBar.gameObject.activeInHierarchy && _currentProgress / 1f < 1f) _progressBar.gameObject.SetActive(true);
    }

    private void UpdateProgress(TeamData occupyingTeam)
    {
        if (_ownerTeam == occupyingTeam)
        {
            if (_currentProgress < 1f) IncreaseProgress(_captureSpeed);
        }
        else if (_ownerTeam != _neutralTeamOwner)
        {
            if (_currentProgress == 0f)
            {
                _currentOccupyingTeam = occupyingTeam;
                UpdateProgressBarColor(occupyingTeam);
                OnObjectiveLost?.Invoke(_ownerTeam);
                SetNewOwner(_neutralTeamOwner);
            }
            else DecreaseProgress(_captureSpeed);
        }
        else
        {
            // owner team is neutral
            if (_currentProgress == 1f)
            {
                SetNewOwner(occupyingTeam);
                OnObjectiveCaptured?.Invoke(_ownerTeam);
            }
            else
            {
                if (occupyingTeam != _currentOccupyingTeam)
                {
                    if (_currentProgress == 0f)
                    {
                        _currentOccupyingTeam = occupyingTeam;
                        UpdateProgressBarColor(occupyingTeam);
                    }
                    else DecreaseProgress(_captureSpeed);
                }
                else IncreaseProgress(_captureSpeed);
            }
        }
    }

    private void RegenerateProgress()
    {
        if (_ownerTeam == _neutralTeamOwner)
        {
            if (_currentProgress > 0f)
            {
                DecreaseProgress(_captureRegenSpeed);
            }
        }
        else if (_currentProgress < 1f) IncreaseProgress(_captureRegenSpeed);
    }

    // This method changes the color of the progress bar to indicate which team is capturing the objective (from neutral and/or 0% progress).
    private void UpdateProgressBarColor(TeamData newTeam)
    {
        /*
        Color newColor = newTeam.TeamColor;
        if (_preserveAlphaTransparency)
        {
            Color teamColor = newTeam.TeamColor;
            newColor = new Color(teamColor.r, teamColor.g, teamColor.b, _progressBarTeamColor.color.a);
        }
        _progressBarTeamColor.color = newColor;*/
        _progressBarTeamColor.color = newTeam.TeamColor;
    }

    // This method changes the colors of the objective itself to indicate which team owns the objective.
    private void SetNewOwner(TeamData newTeam)
    {
        _ownerTeam = newTeam;
        // Change objective team colors.
        Color newColor = newTeam.TeamColor;
        foreach (SpriteRenderer renderer in _objectiveColors)
        {
            if (_preserveAlphaTransparency)
            {
                Color teamColor = newTeam.TeamColor;
                newColor = new Color(teamColor.r, teamColor.g, teamColor.b, renderer.color.a);
            }
            renderer.color = newColor;
        }
    }

    private void IncreaseProgress(float progressSpeed)
    {
        _currentProgress = Mathf.Clamp01(_currentProgress + (progressSpeed * Time.deltaTime));
        _progressBar.UpdateSliderValue(_currentProgress);
    }

    private void DecreaseProgress(float progressSpeed)
    {
        _currentProgress = Mathf.Clamp01(_currentProgress - (progressSpeed * Time.deltaTime));
        _progressBar.UpdateSliderValue(_currentProgress);
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