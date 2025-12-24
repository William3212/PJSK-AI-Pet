public class AIResponse
{
    public string text;
    public Emotion? emotion; // ← 可为空

    public AIResponse(string t, Emotion? e)
    {
        text = t;
        emotion = e;
    }
}
