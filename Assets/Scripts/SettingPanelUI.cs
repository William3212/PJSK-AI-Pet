using TMPro;
using UnityEngine;

public class SettingPanelUI : MonoBehaviour
{
    [Header("Inputs")]
    public TMP_InputField promptInput;
    public TMP_InputField apiUrlInput;
    public TMP_InputField apiKeyInput;

    public CharacterManager characterManager;
    public DeskPetAnimator animator;
    public DeepSeekClient deepSeekClient;

    void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        // API
        apiUrlInput.text = SettingsStorage.LoadApiUrl();
        apiKeyInput.text = SettingsStorage.LoadApiKey();

        if (characterManager.CurrentCharacter == null) return;

        promptInput.SetTextWithoutNotify(
            characterManager.CurrentCharacter.runtimeAIPrompt
        );
    }

    // ⭐ 保存按钮
    public void OnSaveClicked()
    {
        var character = characterManager.CurrentCharacter;
        string name = character.characterName;

        // 保存 API
        SettingsStorage.SaveApi(
            apiUrlInput.text,
            apiKeyInput.text);

        deepSeekClient.ApplyApiSetting(
            apiUrlInput.text,
            apiKeyInput.text);

        // 保存 Prompt
        character.runtimeAIPrompt = promptInput.text;
        SettingsStorage.SaveCharacterPrompt(name, promptInput.text);

        // 保存性别 & 风格
        SettingsStorage.SaveCharacterGender(
            name, (int)animator.gender);

        int style =
            animator.gender == Gender.Male
            ? (int)animator.maleStyle
            : (int)animator.femaleStyle;

        SettingsStorage.SaveCharacterStyle(name, style);

        Debug.Log("设置已保存");
    }
}
