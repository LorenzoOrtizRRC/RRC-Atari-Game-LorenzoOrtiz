using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleApplyStartColors : MonoBehaviour
{
    [SerializeField] private ParticleSystem _selfParticleSystem;
    [SerializeField] private ParticleSystem[] _childParticleSystems = new ParticleSystem[0];

    private void Start()
    {
        foreach (ParticleSystem childParticleSystem in _childParticleSystems)
        {
            ParticleSystem.MainModule psMain = childParticleSystem.main;
            psMain.startColor = _selfParticleSystem.main.startColor;
        }
    }
}
