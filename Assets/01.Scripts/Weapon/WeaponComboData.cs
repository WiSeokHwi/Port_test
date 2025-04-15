using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ComboData", menuName = "Weapon/Combo Data")]
public class WeaponComboData : ScriptableObject
{
    public string subStateMachineName; // 예: "AttackAxe"
    public List<AnimationClip> comboStateNames; // 예: "Attack1", "Attack2", ...
    public int maxComboCount => comboStateNames.Count; // 최대 콤보 수 계산

    public List<int> GetStateHashes(string layerName = "Base Layer")
    {
        List<int> hashes = new List<int>();
        foreach (var _name in comboStateNames)
        {
            string fullPath = $"{layerName}.{subStateMachineName}.{_name.name}";
            hashes.Add(Animator.StringToHash(fullPath));
        }
        return hashes;
    }
    
}

