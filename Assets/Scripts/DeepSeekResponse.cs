[System.Serializable]
public class DeepSeekResponse
{
    public Choice[] choices;
}

[System.Serializable]
public class Choice
{
    public Message message;
}
