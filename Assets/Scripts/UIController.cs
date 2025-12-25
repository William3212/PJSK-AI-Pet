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

        SyncDropdownsFromCharacter();
    }


    void InitCharacterDropdown()
    {
        characterDropdown.ClearOptions();
        List<string> names = new();

        foreach (var c in characterManager.characters)
            names.Add(c.characterName);

        characterDropdown.AddOptions(names);
    }

    void InitGenderDropdown()
    {
        genderDropdown.AddOptions(new List<string> { "Male", "Female" });
    }

    void OnCharacterChanged(int index)
    {
        characterManager.SwitchCharacter(index);

        var character = characterManager.CurrentCharacter;

        // 同步性别下拉框

        genderDropdown.value = (int)character.runtimeGender;
        

        // 同步风格下拉框
 
        if (character.runtimeGender == Gender.Male)
            styleDropdown.value = character.runtimeStyle;
        else
            styleDropdown.value = character.runtimeStyle;

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
                    "Happy", "Pure"
                }
            );
        }

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



    void SyncDropdownsFromCharacter()
    {
        var character = characterManager.CurrentCharacter;
        if (character == null) return;

        // 角色下拉框
        int index = characterManager.characters
            .FindIndex(c => c == character);

        if (index >= 0)
            characterDropdown.SetValueWithoutNotify(index);

        // 性别
        genderDropdown.SetValueWithoutNotify((int)character.runtimeGender);

        // 风格
        RefreshStyleDropdown(character.runtimeGender);
        styleDropdown.SetValueWithoutNotify(character.runtimeStyle);
    }


}
