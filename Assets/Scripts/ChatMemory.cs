using System.Collections.Generic;

[System.Serializable]
public class ChatMemory
{
    public List<Message> messages = new();

    public void Clear()
    {
        messages.Clear();
    }

    public void AddUser(string text)
    {
        messages.Add(new Message
        {
            role = "user",
            content = text
        });
    }

    public void AddAssistant(string text)
    {
        messages.Add(new Message
        {
            role = "assistant",
            content = text
        });
    }
}
