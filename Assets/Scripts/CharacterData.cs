using Spine.Unity;

[System.Serializable]
public class CharacterData
{
    public string characterName;
    public SkeletonDataAsset skeletonDataAsset;

    public Gender defaultGender;
    public MaleStyle defaultMaleStyle;
    public FemaleStyle defaultFemaleStyle;

    // ⭐ AI Prompt
    public string runtimeAIPrompt;
}
