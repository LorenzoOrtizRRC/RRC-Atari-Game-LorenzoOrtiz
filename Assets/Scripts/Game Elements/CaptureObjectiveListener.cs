using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureObjectiveListener : MonoBehaviour
{
    // This script is responsible for re-assigning Waypoints to spawners when the capture objective's owner team changes.
    // Without this script, spawners cannot interact with Capture Objectives.
    [SerializeField] private TeamData _teamReference;
    [SerializeField] private CaptureObjective _captureObjective;
    [SerializeField] private MinionSpawner[] _affectedSpawners;
    [SerializeField] private WaypointPath _newPath;

    // Start is called before the first frame update
    private void Awake()
    {
        //_captureObjective.OnObjectiveCaptured.AddListener(UpdateSpawnerWaypoints);
    }

    private void UpdateSpawnerWaypoints(TeamData newOwnerTeam)
    {
        if (newOwnerTeam == _teamReference)
        {
            foreach (MinionSpawner spawner in _affectedSpawners)
            {
                spawner.SetTeam(newOwnerTeam);
                spawner.SetWaypointPath(_newPath);
            }
        }
    }
}
