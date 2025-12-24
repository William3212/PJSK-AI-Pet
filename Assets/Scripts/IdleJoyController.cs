using UnityEngine;

public class IdleJoyController : MonoBehaviour
{
    public DeskPetAnimator animator;

    public float idleTimeToJoy = 120f;

    float idleTimer = 0f;
    bool joyTriggered = false;

    void Update()
    {
        if (animator == null) return;

        if (animator.IsBusy())
        {
            idleTimer = 0f;
            joyTriggered = false;
            return;
        }

        idleTimer += Time.deltaTime;

        if (!joyTriggered && idleTimer >= idleTimeToJoy)
        {
            joyTriggered = true;
            idleTimer = 0f;
            animator.PlayJoyOnce();
        }
    }

    public void ResetTimer()
    {
        idleTimer = 0f;
        joyTriggered = false;
    }
}
