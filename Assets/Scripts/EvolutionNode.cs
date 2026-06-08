using UnityEngine;

[CreateAssetMenu(fileName = "NewEvolutionNode", menuName = "Stick Evolution/Evolution Node")]
public class EvolutionNode : ScriptableObject
{
    
    public int tier = 1;
public string stageName;
    [TextArea]
    public string description;
    public long requiredResources;
    public int resourcePerClick;
    public Sprite stageSprite;
    public EvolutionNode[] nextPossibleEvolutions;
}
