using UnityEngine;

public class PetDrag : MonoBehaviour
{
    private Vector3 offset;
    private Camera mainCamera;
    private bool isDragging = false;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        isDragging = true;

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        offset = transform.position - mouseWorldPos;
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        transform.position = mouseWorldPos + offset;
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(mainCamera.transform.position.z);
        return mainCamera.ScreenToWorldPoint(mouseScreenPos);
    }
}
