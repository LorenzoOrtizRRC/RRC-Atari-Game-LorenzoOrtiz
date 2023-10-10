using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionListener : MonoBehaviour
{
    public Action OnMissionSuccess;
    public Action OnMissionFailure;

    public void TriggerMissionSuccess()
    {
        OnMissionSuccess?.Invoke();
    }

    public void TriggerMissionFailure()
    {
        OnMissionFailure?.Invoke();
    }
}
