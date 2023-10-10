using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MissionObjective : MonoBehaviour
{
    public Action<int, int> OnMissionObjectiveUpdated;

    [SerializeField] private List<MissionListener> _missionListeners = new List<MissionListener>();
    private int _currentProgress = 0;
    private int _maxProgress = 0;       // When _currentProgress = _maxProgress, this mission is finished.

    public int CurrentProgress => _currentProgress;
    public int MaxProgress => _maxProgress;

    private void Start()
    {
        foreach (MissionListener missionListener in _missionListeners)
        {
            //missionListener.TriggerMissionSuccess += AddMissionSuccess;
        }
    }

    public void AddMissionSuccess()
    {
        _currentProgress++;
        OnMissionObjectiveUpdated?.Invoke(_currentProgress, _maxProgress);
    }

    public void AddMissionFailure()
    {
        _currentProgress--;
        OnMissionObjectiveUpdated?.Invoke(_currentProgress, _maxProgress);
    }
}
