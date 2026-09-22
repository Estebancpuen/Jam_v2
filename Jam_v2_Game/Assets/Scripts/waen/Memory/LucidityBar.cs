using UnityEngine;
using UnityEngine.UI;

public class LucidityBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private LucidityController lucidity;

    private void Start()
    {
        lucidity =
            FindFirstObjectByType<LucidityController>();

        if (lucidity == null)
        {
            Debug.LogWarning("No se encontró LucidityController.");
            return;
        }

        lucidity.OnLucidityChanged.AddListener(UpdateBar);

        UpdateBar(lucidity.CurrentLucidity);
    }

    private void UpdateBar(float currentLucidity)
    {
        fillImage.fillAmount =
            currentLucidity / lucidity.MaxLucidity;
    }

    private void OnDestroy()
    {
        if (lucidity != null)
        {
            lucidity.OnLucidityChanged.RemoveListener(UpdateBar);
        }
    }
}