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

    [Header("Idle Joy")]
    public IdleJoyController idleJoyController;

    // ===================== 映射表 =====================

    Dictionary<Emotion, string> emotionKeyMap = new Dictionary<Emotion, string>()
    {
        { Emotion.Laugh, "laugh" },
        { Emotion.Angry, "angry" },
        { Emotion.Sad, "sad" },
        { Emotion.Surprise, "surprise" },
        { Emotion.Doubt, "doubt" },
    };

    Dictionary<ActionAnim, string> actionKeyMap = new Dictionary<ActionAnim, string>()
    {
        { ActionAnim.Idle, "idle" },
        { ActionAnim.Listen, "listen" },
        { ActionAnim.Talk, "talk" }
    };

    // ===================== Unity =====================

    void Start()
    {
        if (skeletonAnimation == null)
            skeletonAnimation = GetComponent<SkeletonAnimation>();

        PlayIdle();
    }

    void OnValidate()
    {
        if (!Application.isPlaying) return;
        PlayEmotion(emotion);
    }

    // ===================== 外部调用 =====================

    /// <summary>
    /// AI 回复（情绪 → talk → idle）
    /// </summary>
    public void PlayAIResponse(Emotion? emotion)
    {
        if (skeletonAnimation == null) return;

        var state = skeletonAnimation.AnimationState;
        state.ClearTracks();

        // 打断 idle joy 计时
        if (idleJoyController != null)
            idleJoyController.ResetTimer();

        // ① 有情绪
        if (emotion.HasValue)
        {
            string emotionAnim = GetEmotionAnimationName(emotion.Value);
            state.SetAnimation(0, emotionAnim, false);

            string talkAnim = GetActionAnimationName(ActionAnim.Talk);
            state.AddAnimation(0, talkAnim, false, 0);
        }
        // ② 没情绪：只 talk
        else
        {
            string talkAnim = GetActionAnimationName(ActionAnim.Talk);
            state.SetAnimation(0, talkAnim, false);
        }

        // ③ 回 idle
        string idleAnim = GetActionAnimationName(ActionAnim.Idle);
        state.AddAnimation(0, idleAnim, true, 0);
    }

    public void PlayEmotion(Emotion e)
    {
        if (skeletonAnimation == null) return;

        if (!emotionKeyMap.ContainsKey(e)) return;

        string animName = GetEmotionAnimationName(e);
        skeletonAnimation.AnimationState.SetAnimation(0, animName, true);
    }

    public void PlayIdle()
    {
        if (skeletonAnimation == null) return;

        string anim = GetActionAnimationName(ActionAnim.Idle);
        skeletonAnimation.AnimationState.SetAnimation(0, anim, true);
    }

    public void PlayListen()
    {
        if (skeletonAnimation == null) return;

        string anim = GetActionAnimationName(ActionAnim.Listen);
        skeletonAnimation.AnimationState.SetAnimation(0, anim, true);

        if (idleJoyController != null)
            idleJoyController.ResetTimer();
    }

    /// <summary>
    /// IdleJoyController 专用：只播放一次 joy
    /// </summary>
    public void PlayJoyOnce()
    {
        if (skeletonAnimation == null) return;

        var state = skeletonAnimation.AnimationState;
        state.ClearTracks();

        string joyAnim = GetJoyAnimationName();
        string idleAnim = GetActionAnimationName(ActionAnim.Idle);

        // joy 播一次
        state.SetAnimation(0, joyAnim, false);
        // 接回 idle
        state.AddAnimation(0, idleAnim, true, 0);
    }


    /// <summary>
    /// 当前是否“忙”（非 idle）
    /// </summary>
    public bool IsBusy()
    {
        var track = skeletonAnimation.AnimationState.GetCurrent(0);
        if (track == null) return false;

        // 当前不是 idle 就认为是 busy
        return !track.Animation.Name.Contains("_idle");
    }

    // ===================== 动画名拼接 =====================

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
        string key = emotionKeyMap[e];

        if (gender == Gender.Male)
            return $"m_{maleStyle.ToString().ToLower()}_{key}01_f_v2";
        else
            return $"w_{femaleStyle.ToString().ToLower()}_{key}01_f_v2";
    }

    string GetJoyAnimationName()
    {
        if (gender == Gender.Male)
            return $"m_{maleStyle.ToString().ToLower()}_joy01_f_v2";
        else
            return $"w_{femaleStyle.ToString().ToLower()}_joy01_f_v2";
    }
}
