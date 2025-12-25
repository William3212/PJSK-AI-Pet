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
        string lastCharacter = SettingsStorage.LoadLastCharacter();

        int index = characterManager.characters
            .FindIndex(c => c.characterName == lastCharacter);

        if (index < 0)
            index = 0;

        // 只做一次切换（这是唯一入口）
        characterManager.SwitchCharacter(index);

        // 同步 UI（不触发事件）
        uiController.SetCharacterDropdownWithoutNotify(index);
    }


    void RefreshAllUI()
    {
        settingPanelUI.RefreshUI();
    }
}
