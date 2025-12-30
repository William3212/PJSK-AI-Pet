using System.IO;
using UnityEngine;

public static class ChatMemoryStorage
{
    static string BasePath =>
        Path.Combine(Application.persistentDataPath, "chat_memory");

    static string GetPath(string characterName)
    {
        return Path.Combine(BasePath, $"{characterName}.json");
    }

    // ================= 保存 =================

    public static void Save(string characterName, ChatMemory memory)
    {
        if (!Directory.Exists(BasePath))
            Directory.CreateDirectory(BasePath);

        string json = JsonUtility.ToJson(memory, true);
        File.WriteAllText(GetPath(characterName), json);
    }

    // ================= 读取 =================

    public static ChatMemory Load(string characterName)
    {
        string path = GetPath(characterName);

        if (!File.Exists(path))
            return new ChatMemory();

        try
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<ChatMemory>(json);
        }
        catch
        {
            return new ChatMemory();
        }
    }

    // ================= 删除 =================

    public static void Clear(string characterName)
    {
        string path = GetPath(characterName);

        if (File.Exists(path))
            File.Delete(path);
    }
}
