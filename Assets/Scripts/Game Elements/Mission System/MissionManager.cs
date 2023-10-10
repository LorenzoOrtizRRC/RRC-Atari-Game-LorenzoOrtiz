using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    [SerializeField] private List<MissionObjective> _missions = new List<MissionObjective>();

    private void Start()
    {
        foreach (MissionObjective mission in _missions)
        {
            mission.Initialize();
        }
    }
}
