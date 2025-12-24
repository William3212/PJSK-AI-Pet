using UnityEngine;
using Spine.Unity;
using System.Collections.Generic;

public enum ActionAnim
{
    Idle,
    Listen,
    Talk
}


public class DeskPetAnimator : MonoBehaviour
{
    [Header("Spine")]
    public SkeletonAnimation skeletonAnimation;

    [Header("Character")]
    public Gender gender;

    public MaleStyle maleStyle;
    public FemaleStyle femaleStyle;

    [Header("Emotion")]
    public Emotion emotion;

    Dictionary<Emotion, string> emotionKeyMap = new Dictionary<Emotion, string>()
    {
        { Emotion.Joy, "joy" },
        { Emotion.Laugh, "laugh" },
        { Emotion.Angry, "angry" },
        { Emotion.Sad, "sad" },
        { Emotion.Surprise, "surprise" },
        { Emotion.Doubt, "doubt" },
    };

    void Start()
    {
        if (skeletonAnimation == null)
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
        }

        PlayIdle();
    }

    void OnValidate()
    {
        if (!Application.isPlaying) return;
        PlayEmotion(emotion);
    }

    public void PlayEmotion(Emotion e)
    {
        if (skeletonAnimation == null) return;

        string animName = GetEmotionAnimationName(e);
        if (string.IsNullOrEmpty(animName)) return;

        skeletonAnimation.AnimationState.SetAnimation(0, animName, true);
    }


    public void PlayAIResponse(Emotion? emotion)
    {
        if (skeletonAnimation == null) return;

        var state = skeletonAnimation.AnimationState;
        state.ClearTracks();

        // ① 有情绪：先播情绪
        if (emotion.HasValue)
        {
            string emotionAnim = GetEmotionAnimationName(emotion.Value);
            state.SetAnimation(0, emotionAnim, false);

            // 再播 Talk
            string talkAnim = GetActionAnimationName(ActionAnim.Talk);
            state.AddAnimation(0, talkAnim, false, 0);
        }
        // ② 没有情绪：只播 Talk
        else
        {
            string talkAnim = GetActionAnimationName(ActionAnim.Talk);
            state.SetAnimation(0, talkAnim, false);
        }

        // ③ 最后回 Idle
        string idleAnim = GetActionAnimationName(ActionAnim.Idle);
        state.AddAnimation(0, idleAnim, true, 0);
    }


    public void PlayIdle()
    {
        if (skeletonAnimation == null) return;

        string idleAnim = GetActionAnimationName(ActionAnim.Idle);
        skeletonAnimation.AnimationState.SetAnimation(0, idleAnim, true);
    }

    Dictionary<ActionAnim, string> actionKeyMap = new()
    {
        { ActionAnim.Idle, "idle" },
        { ActionAnim.Listen, "listen" },
        { ActionAnim.Talk, "talk" }
    };

    public void PlayListen()
    {
        if (skeletonAnimation == null) return;

        string anim = GetActionAnimationName(ActionAnim.Listen);
        skeletonAnimation.AnimationState.SetAnimation(0, anim, true);
    }

    public void PlayTalkOnce()
    {
        if (skeletonAnimation == null) return;

        string anim = GetActionAnimationName(ActionAnim.Talk);
        skeletonAnimation.AnimationState.SetAnimation(0, anim, false);
    }

    string GetActionAnimationName(ActionAnim action)
    {
        string key = actionKeyMap[action];

        if (gender == Gender.Male)
            return $"m_{maleStyle.ToString().ToLower()}_{key}01_f_v2";
        else
            return $"w_{femaleStyle.ToString().ToLower()}_{key}01_f_v2";
    }
    
    string GetEmotionAnimationName(Emotion e)
    {
        string emotionKey = emotionKeyMap[e];

        if (gender == Gender.Male)
        {
            return $"m_{maleStyle.ToString().ToLower()}_{emotionKey}01_f_v2";
        }
        else
        {
            return $"w_{femaleStyle.ToString().ToLower()}_{emotionKey}01_f_v2";
        }
    }
}
