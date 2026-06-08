using UnityEngine;

[CreateAssetMenu(fileName = "NewMaterial", menuName = "Stick Evolution/Material Data")]
public class MaterialData : ScriptableObject
{
    public string materialName;
    public string description;
    public Sprite icon;
    public float dropProbability = 0.05f;
}
