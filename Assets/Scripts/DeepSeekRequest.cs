[System.Serializable]
public class DeepSeekRequest
{
    public string model;
    public Message[] messages;
}

[System.Serializable]
public class Message
{
    public string role;
    public string content;
}
