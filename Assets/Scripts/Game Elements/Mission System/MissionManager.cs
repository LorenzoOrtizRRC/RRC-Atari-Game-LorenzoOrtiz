using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MissionManager : MonoBehaviour
{
    public UnityEvent OnMissionsCompleted;
    public UnityEvent OnMissionsFailed;

    [SerializeField] private MissionDisplayManager _missionDisplayManager;
    [SerializeField] private List<MissionObjective> _missions = new List<MissionObjective>();

    // Initialize mission objectives.
    // Create mission UI.
    // Create mission objective UI elements based on number of missions. Put them in a list.
    // Update mission objective UI text if the corresponding mission objective is updated.
    // Send an event when all missions are successful.
    // Send an event when all missions are failed.

    private void Start()
    {
        foreach (MissionObjective missionObjective in _missions)
        {
            missionObjective.Initialize();
            missionObjective.OnMissionUpdated += EvaluateMissions;
            missionObjective.OnMissionUpdated += UpdateMissionDisplay;
        }

        _missionDisplayManager.Initialize(_missions);
    }

    private void EvaluateMissions(MissionObjective mission)
    {
        // If equals to negative of total mission count or less, player lost. If equals to total mission count, player wins.
        int objectiveCompletion = 0;
        foreach (MissionObjective objective in _missions)
        {
            objectiveCompletion += objective.GetCompletionStatus();
        }
        if (objectiveCompletion == _missions.Count) OnMissionsCompleted?.Invoke();
        else if (objectiveCompletion <= -_missions.Count) OnMissionsFailed?.Invoke();
    }

    private void UpdateMissionDisplay(MissionObjective mission)
    {
        int missionIndex = _missions.IndexOf(mission);
        _missionDisplayManager.UpdateMissionDisplay(mission, missionIndex);
    }
}
