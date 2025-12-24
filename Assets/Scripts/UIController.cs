using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{
    public TMP_Dropdown characterDropdown;
    public TMP_Dropdown genderDropdown;
    public TMP_Dropdown styleDropdown;

    public CharacterManager characterManager;
    public DeskPetAnimator petAnimator;
    public SettingPanelUI settingPanelUI;


    void Start()
    {
        InitCharacterDropdown();
        InitGenderDropdown();

        characterDropdown.onValueChanged.AddListener(OnCharacterChanged);
        genderDropdown.onValueChanged.AddListener(OnGenderChanged);
        styleDropdown.onValueChanged.AddListener(OnStyleChanged);

        // 初始化
        petAnimator.gender = (Gender)genderDropdown.value;
        RefreshStyleDropdown(petAnimator.gender);
        //启动时恢复“上一次角色”
        string last = SettingsStorage.LoadLastCharacter();
        int index = characterManager.characters
            .FindIndex(c => c.characterName == last);

        if (index < 0) index = 0;
        characterDropdown.value = index;
        characterManager.SwitchCharacter(index);

    }


    void InitCharacterDropdown()
    {
        characterDropdown.ClearOptions();
        List<string> names = new();

        foreach (var c in characterManager.characters)
            names.Add(c.characterName);

        characterDropdown.AddOptions(names);
        OnCharacterChanged(0);
    }

    void InitGenderDropdown()
    {
        genderDropdown.ClearOptions();
        genderDropdown.AddOptions(new List<string> { "Male", "Female" });
    }

    void OnCharacterChanged(int index)
    {
        characterManager.SwitchCharacter(index);

        var character = characterManager.CurrentCharacter;

        // 同步性别下拉框
        genderDropdown.value = (int)character.defaultGender;

        // 刷新风格列表
        RefreshStyleDropdown(character.defaultGender);

        // 同步风格下拉框
        if (character.defaultGender == Gender.Male)
            styleDropdown.value = (int)character.defaultMaleStyle;
        else
            styleDropdown.value = (int)character.defaultFemaleStyle;

        OnStyleChanged(styleDropdown.value);
        settingPanelUI.RefreshUI();
    }



    void OnGenderChanged(int index)
    {
        Gender gender = (Gender)index;
        petAnimator.gender = gender;

        RefreshStyleDropdown(gender);
    }


    void RefreshStyleDropdown(Gender gender)
    {
        styleDropdown.ClearOptions();

        if (gender == Gender.Male)
        {
            styleDropdown.AddOptions(
                new List<string> { "Cool", "Happy", "Normal" }
            );
        }
        else
        {
            styleDropdown.AddOptions(
                new List<string>
                {
                    "Adult", "Cool", "Cute",
                    "Happy", "KohaneUnit", "Pure"
                }
            );
        }

        styleDropdown.value = 0;
        OnStyleChanged(0);
    }

    void OnStyleChanged(int index)
    {
        if (petAnimator.gender == Gender.Male)
            petAnimator.maleStyle = (MaleStyle)index;
        else
            petAnimator.femaleStyle = (FemaleStyle)index;

        petAnimator.PlayIdle();
    }
    public void SetCharacterDropdownWithoutNotify(int index)
    {
        characterDropdown.SetValueWithoutNotify(index);
    }
    public void ApplySavedGenderAndStyle()
    {
        var character = characterManager.CurrentCharacter;
        string name = character.characterName;

        // 性别
        int gender = SettingsStorage.LoadCharacterGender(
            name, (int)character.defaultGender);

        genderDropdown.SetValueWithoutNotify(gender);
        petAnimator.gender = (Gender)gender;

        // 风格
        RefreshStyleDropdown(petAnimator.gender);

        int style = SettingsStorage.LoadCharacterStyle(
            name,
            petAnimator.gender == Gender.Male
                ? (int)character.defaultMaleStyle
                : (int)character.defaultFemaleStyle
        );

        styleDropdown.SetValueWithoutNotify(style);
        OnStyleChanged(style);
    }
    public void RefreshAllDropdowns()
    {
        RefreshStyleDropdown(petAnimator.gender);
    }

}
