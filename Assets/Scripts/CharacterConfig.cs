using UnityEngine;
using Spine.Unity;

[CreateAssetMenu(
    fileName = "CharacterConfig",
    menuName = "DeskPet/Character Config"
)]
public class CharacterConfig : ScriptableObject
{
    [Header("Basic")]
    public string displayName;
    public SkeletonDataAsset skeletonData;

    [Header("Default Settings")]
    public Gender defaultGender;
    public MaleStyle defaultMaleStyle;
    public FemaleStyle defaultFemaleStyle;

    [Header("AI Personality")]
    [TextArea(3, 10)]
    public string defaultAIPrompt;
}

