using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionDisplayManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private MissionDisplay _missionDisplayUIPrefab;
    [SerializeField] private GameObject _missionDisplayWindowParent;

    private List<MissionDisplay> _missionDisplayList;

    public void Initialize(List<MissionObjective> objectives)
    {
        _missionDisplayList = new List<MissionDisplay>(objectives.Count);
        for (int i = 0; i < objectives.Count; i++)
        {
            MissionDisplay currentDisplay = Instantiate(_missionDisplayUIPrefab, _missionDisplayWindowParent.transform);
            currentDisplay.MissionTitle.text = objectives[i].MissionTitle;
            _missionDisplayList.Add(currentDisplay);
            UpdateMissionDisplay(objectives[i], i);
        }
    }

    public void UpdateMissionDisplay(MissionObjective objective, int missionIndex)
    {
        // If objective's mission isn't singular (e.g. defeat 5 enemies), add text to match this.
        string newDescription = objective.MissionDescription;
        if (objective.MaxProgress > 1)
        {
            newDescription += $" ({objective.CurrentProgress}/{objective.MaxProgress})";
        }
        _missionDisplayList[missionIndex].MissionDescription.text = newDescription;
    }
}
