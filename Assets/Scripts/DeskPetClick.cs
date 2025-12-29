using UnityEngine;

public class DeskPetClick : MonoBehaviour
{
    public DialogBubbleController dialog;

    void OnMouseDown()
    {
        if (dialog != null)
        {
            dialog.ShowInput();
        }
    }
}
