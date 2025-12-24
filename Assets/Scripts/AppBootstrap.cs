using UnityEngine;

public class AppBootstrap : MonoBehaviour
{
    public CharacterManager characterManager;
    public UIController uiController;
    public SettingPanelUI settingPanelUI;
    public DeepSeekClient deepSeekClient;

    void Start()
    {
        LoadGlobalSettings();
        LoadLastCharacterAndState();
        RefreshAllUI();
    }

    void LoadGlobalSettings()
    {
        // ⭐ API 设置
        string url = SettingsStorage.LoadApiUrl();
        string key = SettingsStorage.LoadApiKey();

        deepSeekClient.ApplyApiSetting(url, key);
    }

    void LoadLastCharacterAndState()
    {
        // ⭐ 找上一次角色
        string lastCharacter =
            SettingsStorage.LoadLastCharacter();

        int index = characterManager.characters
            .FindIndex(c => c.characterName == lastCharacter);

        if (index < 0) index = 0;

        // ⭐ 切角色（这里会加载默认 / 保存值）
        uiController.SetCharacterDropdownWithoutNotify(index);
        characterManager.SwitchCharacter(index);

        // ⭐ 恢复性别 & 风格（UI + Animator）
        uiController.ApplySavedGenderAndStyle();
    }

    void RefreshAllUI()
    {
        uiController.RefreshAllDropdowns();
        settingPanelUI.RefreshUI();
    }
}
