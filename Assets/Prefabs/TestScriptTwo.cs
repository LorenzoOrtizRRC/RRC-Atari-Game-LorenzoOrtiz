using UnityEngine;

[System.Serializable]
public abstract class Weapon
{
    [SerializeField]
    private string weaponName;

    // Other fields and methods...
}

[System.Serializable]
public class Pistol : Weapon
{
    [SerializeField]
    private int ammoCapacity;

    // Other fields and methods...
}

public class TestScriptTwo : MonoBehaviour
{
    [SerializeField]
    private Pistol equippedPistol;

    // Other fields and methods....
}
