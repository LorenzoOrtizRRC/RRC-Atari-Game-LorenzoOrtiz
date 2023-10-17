using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerObjective : CaptureObjective
{
    // When captured, adds minions to its ownerTeam's spawners in _affectedMinionSpawners.
    // When lost (neutral), removes minions from its previous ownerTeam's spawners in _affectedMinionSpawners.
    [SerializeField] private List<MinionSpawner> _affectedMinionSpawners;
    [SerializeField] private List<CharacterAgent> _affectedAgents;
    [SerializeField] private List<SpawnerData> _minionsToAdd;

    protected override void Awake()
    {
        base.Awake();

        OnObjectiveCaptured.AddListener(AddMinionsToSpawner);
        OnObjectiveCaptured.AddListener(SetAffectedTeams);
        OnObjectiveLost.AddListener(RemoveMinionsFromSpawner);
        OnObjectiveLost.AddListener(SetAffectedTeamsNeutral);
        if (OwnerTeam != NeutralTeamOwner)
        {
            foreach (MinionSpawner minionSpawner in _affectedMinionSpawners)
            {
                if (minionSpawner.SpawnerTeam == OwnerTeam)
                {
                    foreach (SpawnerData minionData in _minionsToAdd)
                    {
                        minionSpawner.AddMinionsInWave(minionData);
                    }
                }
            }
        }
    }

    public void SetAffectedTeams(TeamData newOwnerTeam)
    {
        foreach (CharacterAgent agent in _affectedAgents)
        {
            agent.SetTeam(newOwnerTeam);
        }

        foreach (MinionSpawner spawner in _affectedMinionSpawners)
        {
            spawner.SetTeam(newOwnerTeam);
        }
    }

    public void SetAffectedTeamsNeutral(TeamData newOwnerTeam)
    {
        foreach (CharacterAgent agent in _affectedAgents)
        {
            agent.SetTeam(NeutralTeamOwner);
        }

        foreach (MinionSpawner spawner in _affectedMinionSpawners)
        {
            spawner.SetTeam(NeutralTeamOwner);
        }
    }

    public void AddMinionsToSpawner(TeamData newOwnerTeam)
    {
        foreach (MinionSpawner minionSpawner in _affectedMinionSpawners)
        {
            if (minionSpawner.SpawnerTeam == newOwnerTeam)
            {
                foreach (SpawnerData minionData in _minionsToAdd)
                {
                    minionSpawner.AddMinionsInWave(minionData);
                }
            }
        }
    }

    public void RemoveMinionsFromSpawner(TeamData oldOwnerTeam)
    {
        foreach (MinionSpawner minionSpawner in _affectedMinionSpawners)
        {
            if (minionSpawner.SpawnerTeam == oldOwnerTeam)
            {
                foreach (SpawnerData minionData in _minionsToAdd)
                {
                    minionSpawner.RemoveMinionsInWave(minionData);
                }
            }
        }
    }
}
