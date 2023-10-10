using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MissionObjective
{
    public Action<MissionObjective> OnMissionObjectiveUpdated;

    [SerializeField] private List<MissionListener> _missionListeners = new List<MissionListener>();
    private int _currentProgress = 0;
    private int _maxProgress = 0;       // When _currentProgress = _maxProgress, this mission is finished.

    public int CurrentProgress => _currentProgress;
    public int MaxProgress => _maxProgress;
    public bool MissionIsComplete => (CurrentProgress / MaxProgress == 1);

    public void Initialize()
    {
        foreach (MissionListener missionListener in _missionListeners)
        {
            missionListener.OnMissionSuccess += AddMissionSuccess;
            missionListener.OnMissionFailure += AddMissionFailure;
        }
    }

    public void AddMissionSuccess()
    {
        _currentProgress++;
        OnMissionObjectiveUpdated?.Invoke(this);
    }

    public void AddMissionFailure()
    {
        _currentProgress--;
        OnMissionObjectiveUpdated?.Invoke(this);
    }
}
