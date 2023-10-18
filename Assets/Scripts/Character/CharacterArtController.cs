using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterArtController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] _renderers;

    private void Awake()
    {
        //_renderers ??= GetComponent<SpriteRenderer>();
    }

    public void Initialize(TeamData _currentTeam)
    {
        // apply team colors
        if (_renderers.Any())
        {
            foreach (SpriteRenderer renderer in _renderers)
            {
                renderer.color = _currentTeam.TeamColor;
            }
        }
    }
}
