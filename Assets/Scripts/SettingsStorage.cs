using UnityEngine;

public static class SettingsStorage
{
    // ===== 全局 =====
    public static void SaveApi(string url, string key)
    {
        PlayerPrefs.SetString("GLOBAL_API_URL", url);
        PlayerPrefs.SetString("GLOBAL_API_KEY", key);
    }

    public static string LoadApiUrl()
        => PlayerPrefs.GetString("GLOBAL_API_URL", "");

    public static string LoadApiKey()
        => PlayerPrefs.GetString("GLOBAL_API_KEY", "");

    public static void SaveLastCharacter(string name)
    {
        PlayerPrefs.SetString("GLOBAL_LAST_CHARACTER", name);
    }

    public static string LoadLastCharacter()
        => PlayerPrefs.GetString("GLOBAL_LAST_CHARACTER", "");

    // ===== 角色 =====
    static string CharKey(string charName, string field)
        => $"CHAR_{charName}_{field}";

    public static void SaveCharacterPrompt(string charName, string prompt)
    {
        PlayerPrefs.SetString(CharKey(charName, "PROMPT"), prompt);
    }

    public static string LoadCharacterPrompt(string charName, string fallback)
    {
        return PlayerPrefs.GetString(
            CharKey(charName, "PROMPT"), fallback);
    }

    public static void SaveCharacterGender(string charName, int gender)
    {
        PlayerPrefs.SetInt(CharKey(charName, "GENDER"), gender);
    }

    public static int LoadCharacterGender(string charName, int fallback)
    {
        return PlayerPrefs.GetInt(
            CharKey(charName, "GENDER"), fallback);
    }

    public static void SaveCharacterStyle(string charName, int style)
    {
        PlayerPrefs.SetInt(CharKey(charName, "STYLE"), style);
    }

    public static int LoadCharacterStyle(string charName, int fallback)
    {
        return PlayerPrefs.GetInt(
            CharKey(charName, "STYLE"), fallback);
    }
}
