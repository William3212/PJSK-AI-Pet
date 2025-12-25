using Spine.Unity;

[System.Serializable]
public class CharacterData
{
    public string characterName;
    public SkeletonDataAsset skeletonDataAsset;

    public Gender defaultGender;
    public MaleStyle defaultMaleStyle;
    public FemaleStyle defaultFemaleStyle;

    // ===== 运行时 =====
    public Gender runtimeGender;
    public int runtimeStyle;

    public string runtimeAIPrompt;
}
