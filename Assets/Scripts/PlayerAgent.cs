using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAgent : MonoBehaviour
{
    public float moveSpeed;
    public CharacterAgent agent;
    [Header("Component References")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private CharacterArtController _characterArtController;
    [Header("Agent Variables")]
    [SerializeField] private TeamData _currentTeam;
    [Header("State Machine Variables")]
    [SerializeField] private WeaponInstance _weapon;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            _weapon.UseWeaponAuto(agent);
        }
    }

    private void FixedUpdate()
    {
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");
        Vector2 offset = new Vector2(xInput, yInput) * moveSpeed * Time.fixedDeltaTime;
        _rb.MovePosition((Vector2)transform.position + offset);
    }
}
