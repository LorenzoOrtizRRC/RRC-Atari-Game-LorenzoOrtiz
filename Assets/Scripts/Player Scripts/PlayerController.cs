using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterAgent _playerAgent;

    private Camera _playerCamera;
    
    public CharacterAgent PlayerAgent => _playerAgent;

    private void OnEnable()
    {
        // disable any statemachines-related components on player ship. this allows the player to be any ship that AI uses.
        StateMachine stateMachine = _playerAgent.GetComponent<StateMachine>();
        if (stateMachine) stateMachine.enabled = false;
        TargetDetector detector = _playerAgent.GetComponentInChildren<TargetDetector>();
        if (detector) detector.gameObject.SetActive(false);
    }

    private void Start()
    {
        _playerCamera = Camera.main;
    }

    private void Update()
    {
        // rotate weapon towards mouse
        Vector2 mouseWorldPosition = (Vector2)_playerCamera.ScreenToWorldPoint(Input.mousePosition);
        _playerAgent.RotateWeapon(mouseWorldPosition);
        // shoot weapon on input: LMB
        if (Input.GetMouseButton(0)) _playerAgent.UseWeapon(mouseWorldPosition);
    }

    private void FixedUpdate()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        Vector2 offset = _playerAgent.transform.position + (new Vector3(inputX, inputY, 0) * _playerAgent.Speed * Time.fixedDeltaTime);
        _playerAgent.Rb.MovePosition(offset);
    }
}
