using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon/Creat New Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public GameObject weaponPrefab;
    public WeaponComboData comboData;
}