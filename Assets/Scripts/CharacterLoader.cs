using UnityEngine;
using System.Collections.Generic;

public static class CharacterLoader
{
    const string CHARACTER_PATH = "SpineCharacters";

    public static List<CharacterData> LoadAllCharacters()
    {
        List<CharacterData> list = new();

        CharacterConfig[] configs =
            Resources.LoadAll<CharacterConfig>(CHARACTER_PATH);

        foreach (var cfg in configs)
        {
            CharacterData data = new CharacterData
            {
                characterName = cfg.displayName,
                skeletonDataAsset = cfg.skeletonData,

                defaultGender = cfg.defaultGender,
                defaultMaleStyle = cfg.defaultMaleStyle,
                defaultFemaleStyle = cfg.defaultFemaleStyle,

                // ⭐ 用默认 Prompt 初始化
                runtimeAIPrompt = cfg.defaultAIPrompt
            };

            list.Add(data);
        }

        return list;
    }
}
