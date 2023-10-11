using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
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
            missionObjective.OnMissionObjectiveUpdated += EvaluateMissions;
            missionObjective.OnMissionObjectiveUpdated += UpdateMissionDisplay;
        }
    }

    private void EvaluateMissions(MissionObjective missionObjective)
    {
        //
    }

    private void UpdateMissionDisplay(MissionObjective missionObjective)
    {
        int index = _missions.IndexOf(missionObjective);
    }
}
