using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    [Header("Mouse Sensitivity")]
    [SerializeField] private float sensitivity = 0.1f;

    [Header("Horizontal Limits")]
    [SerializeField] private float minYaw = -45f;
    [SerializeField] private float maxYaw = 45f;

    [Header("Vertical Limits")]
    [SerializeField] private float minPitch = -15f;
    [SerializeField] private float maxPitch = 20f;

    private float yaw;
    private float pitch;

    private void Start()
    {
        yaw = 0f;
        pitch = 0f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.CurrentState != GameState.Playing)
            return;

        if (Mouse.current == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * sensitivity;
        float mouseY = mouseDelta.y * sensitivity;

        yaw += mouseX;
        pitch -= mouseY;

        yaw = Mathf.Clamp(yaw, minYaw, maxYaw);
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        cameraTransform.localRotation =
            Quaternion.Euler(pitch, yaw, 0f);
    }
}