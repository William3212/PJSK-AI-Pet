using UnityEngine;
using Spine.Unity;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;

    public List<CharacterData> characters { get; private set; }

    public CharacterData CurrentCharacter { get; private set; }

    void Awake()
    {
        if (skeletonAnimation == null)
            skeletonAnimation = GetComponent<SkeletonAnimation>();

        characters = CharacterLoader.LoadAllCharacters();
    }

public void SwitchCharacter(int index)
    {
        CurrentCharacter = characters[index];

        skeletonAnimation.skeletonDataAsset =
            CurrentCharacter.skeletonDataAsset;
        skeletonAnimation.Initialize(true);

        DeskPetAnimator animator = GetComponent<DeskPetAnimator>();

        string name = CurrentCharacter.characterName;

        // ⭐ 读取性别
        // === 性别 ===
        CurrentCharacter.runtimeGender =
            (Gender)SettingsStorage.LoadCharacterGender(
                name, (int)CurrentCharacter.defaultGender);

        animator.gender = CurrentCharacter.runtimeGender;


        // ⭐ 读取风格
        if (animator.gender == Gender.Male)
        {
            CurrentCharacter.runtimeStyle =
                SettingsStorage.LoadCharacterStyle(
                    name, (int)CurrentCharacter.defaultMaleStyle);
        }
        else
        {
            CurrentCharacter.runtimeStyle =
                SettingsStorage.LoadCharacterStyle(
                    name, (int)CurrentCharacter.defaultFemaleStyle);
        }
        
        // 同步到 Animator
        if (animator.gender == Gender.Male)
            animator.maleStyle = (MaleStyle)CurrentCharacter.runtimeStyle;
        else
            animator.femaleStyle = (FemaleStyle)CurrentCharacter.runtimeStyle;


        // ⭐ 读取 Prompt
        CurrentCharacter.runtimeAIPrompt =
            SettingsStorage.LoadCharacterPrompt(
                name, CurrentCharacter.runtimeAIPrompt);

        SettingsStorage.SaveLastCharacter(name);

        animator.PlayIdle();
    }


}
