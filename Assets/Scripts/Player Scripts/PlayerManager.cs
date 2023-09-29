using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    // This script is responsible for managing the player entity and making related connections for components to and from the player.
    [Header("Component References")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private ResourceBar _playerHealthBar;

    private void Start()
    {
        // initialize values

        // assign events
        _playerController.PlayerAgent.OnHealthDecreased.AddListener(_playerHealthBar.UpdateSliderValue);
    }
}
