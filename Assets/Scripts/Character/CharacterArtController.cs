using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterArtController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;

    private void Awake()
    {
        _renderer ??= GetComponent<SpriteRenderer>();
    }

    public void Initialize(TeamData _currentTeam)
    {
        // apply team colors
        _renderer.color = _currentTeam.TeamColor;
    }
}
