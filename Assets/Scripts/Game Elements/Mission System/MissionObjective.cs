using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum MissionCompletionCondition {ReachedMaxProgress, LessThanMaxProgress, CurrentProgressIsZero}

[Serializable]
public class MissionObjective
{
    public Action<MissionObjective> OnMissionUpdated;

    [SerializeField] private string _missionTitle;
    [SerializeField] private string _missionDescription;
    [SerializeField] private bool _isLoseCondition;
    [SerializeField] private bool _isInstantLoseCondition = false;     // If this mission is failed, immediately lose the game. No win-equivalent.
    [SerializeField] private MissionCompletionCondition _completionCondition = MissionCompletionCondition.ReachedMaxProgress;
    [SerializeField] private List<MissionListener> _missionListeners = new List<MissionListener>();

    private int _currentProgress = 0;       // Initially zero.
    private int _maxProgress = 0;       // When _currentProgress = _maxProgress, this mission is finished.

    public string MissionTitle => _missionTitle;
    public string MissionDescription => _missionDescription;
    public bool IsInstantLoseCondition => _isInstantLoseCondition;
    public int CurrentProgress => _currentProgress;
    public int MaxProgress => _maxProgress;

    public void Initialize()
    {
        foreach (MissionListener missionListener in _missionListeners)
        {
            missionListener.OnMissionSuccess += TrackMissionSuccess;
            missionListener.OnMissionFailure += TrackMissionFailure;
        }
        _maxProgress = _missionListeners.Count;
    }

    // Returns 1 if complete, 0 if incomplete, -1 if failed, and -100 if instant lose.
    public int GetCompletionStatus()
    {
        int completionStatus = 0;
        if (_completionCondition == MissionCompletionCondition.ReachedMaxProgress)
        {
            if (CurrentProgress / MaxProgress >= 1) completionStatus++;
        }
        else if (_completionCondition == MissionCompletionCondition.LessThanMaxProgress)
        {
            if (CurrentProgress < MaxProgress) completionStatus++;
        }
        else if (_completionCondition == MissionCompletionCondition.CurrentProgressIsZero)
        {
            if (CurrentProgress == 0) completionStatus++;
        }
        if (completionStatus > 1) completionStatus = -100;
        else if (_isLoseCondition) completionStatus = -completionStatus;
        return completionStatus;
    }

    // Returns true if this mission objective is complete.
    /*public bool GetCompletionStatus()
    {
        if (_completionCondition == MissionCompletionCondition.ReachedMaxProgress)
        {
            return CurrentProgress / MaxProgress >= 1;
        }
        else if (_completionCondition == MissionCompletionCondition.LessThanMaxProgress)
        {
            return CurrentProgress < MaxProgress;
        }
        else
        {
            return CurrentProgress == 0;
        }
    }*/

    private void TrackMissionSuccess()
    {
        _currentProgress++;
        OnMissionUpdated?.Invoke(this);
    }

    private void TrackMissionFailure()
    {
        _currentProgress--;
        OnMissionUpdated?.Invoke(this);
    }
}
