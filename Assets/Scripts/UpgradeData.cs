using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Stick Evolution/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    public enum UpgradeType { ClickValue, ClickMultiplier, IdleProduction, CriticalChance, CriticalMultiplier }
    public UpgradeType upgradeType = UpgradeType.ClickValue;

    public string upgradeName;
    [TextArea]
    public string description;
    public long baseCost;
    public float costMultiplier = 1.5f;
        public float upgradeValue;

    public float successProbability = 1.0f; // Added probability
    public Sprite icon;

    public long GetCost(int level)
    {
        return (long)(baseCost * Mathf.Pow(costMultiplier, level));
    }
}
