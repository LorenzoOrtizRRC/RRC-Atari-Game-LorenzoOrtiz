using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureMissionListener : MissionListener
{
    [SerializeField] private TeamData _teamReference;
    public void TriggerMissionSuccess(TeamData team)
    {
        if (team == _teamReference) OnMissionSuccess?.Invoke();
    }

    public void TriggerMissionFailure(TeamData oldTeam)
    {
        if (oldTeam == _teamReference) OnMissionFailure?.Invoke();
    }
}
