using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class MemoryInteractor : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera playerCamera;

    [Header("Interaction")]
    [SerializeField] private float interactionDistance = 20f;
    [SerializeField] private LayerMask interactionLayer;

    [Header("UI")]
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private TMP_Text interactionText;

    private Memory currentMemory;

    private void Start()
    {
        HidePrompt();
    }

    private void Update()
    {
        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.CurrentState != GameState.Playing)
        {
            HidePrompt();
            return;
        }

        DetectMemory();

        if (currentMemory != null &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            currentMemory.Interact();

            HidePrompt();
        }
    }

    private void DetectMemory()
    {
        currentMemory = null;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            interactionDistance,
            interactionLayer))
        {
            Memory memory = hit.collider.GetComponent<Memory>();

            if (memory != null)
            {
                currentMemory = memory;

                ShowPrompt(memory);

                return;
            }
        }

        HidePrompt();
    }

    private void ShowPrompt(Memory memory)
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
        }

        if (interactionText != null)
        {
            interactionText.text = "[E] RECORDAR";
        }
    }

    private void HidePrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }
}