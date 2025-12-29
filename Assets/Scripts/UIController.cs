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
        Debug.Log("UIController Start()");

        InitCharacterDropdown();
        InitGenderDropdown();

        // 绑定事件监听器
        characterDropdown.onValueChanged.AddListener(OnCharacterChanged);
        genderDropdown.onValueChanged.AddListener(OnGenderChanged);
        styleDropdown.onValueChanged.AddListener(OnStyleChanged);

        // 延迟同步，确保 AppBootstrap 切换角色已完成
        StartCoroutine(DelayedSync());
    }

    System.Collections.IEnumerator DelayedSync()
    {
        yield return null; // 等一帧
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
        genderDropdown.ClearOptions();
        genderDropdown.AddOptions(new List<string> { "Male", "Female" });
    }

    void OnCharacterChanged(int index)
    {
        Debug.Log($"OnCharacterChanged index={index}");
        characterManager.SwitchCharacter(index);

        var character = characterManager.CurrentCharacter;
        if (character == null) return;

        // 同步性别下拉框
        genderDropdown.SetValueWithoutNotify((int)character.runtimeGender);

        // 同步风格下拉框
        RefreshStyleDropdown(character.runtimeGender);
        styleDropdown.SetValueWithoutNotify(SafeStyleIndex(character.runtimeStyle));

        OnStyleChanged(styleDropdown.value);
        settingPanelUI.RefreshUI();
    }

    void OnGenderChanged(int index)
    {
        Debug.Log($"OnGenderChanged index={index}");
        Gender gender = (Gender)index;
        petAnimator.gender = gender;

        RefreshStyleDropdown(gender);
        styleDropdown.SetValueWithoutNotify(SafeStyleIndex(styleDropdown.value));

        OnStyleChanged(styleDropdown.value);
    }

    void RefreshStyleDropdown(Gender gender)
    {
        styleDropdown.ClearOptions();
        List<string> options = gender == Gender.Male
            ? new List<string> { "Cool", "Happy", "Normal" }
            : new List<string> { "Adult", "Cool", "Cute", "Happy", "Pure" };
        styleDropdown.AddOptions(options);
        Debug.Log($"Style options count = {styleDropdown.options.Count}");
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
        int index = characterManager.characters.FindIndex(c => c == character);
        if (index >= 0)
            characterDropdown.SetValueWithoutNotify(index);

        // 性别下拉框
        genderDropdown.SetValueWithoutNotify((int)character.runtimeGender);

        // 风格下拉框
        RefreshStyleDropdown(character.runtimeGender);
        styleDropdown.SetValueWithoutNotify(SafeStyleIndex(character.runtimeStyle));
    }

    int SafeStyleIndex(int desiredIndex)
    {
        return Mathf.Clamp(desiredIndex, 0, styleDropdown.options.Count - 1);
    }
}
