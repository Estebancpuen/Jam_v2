using UnityEngine;
using UnityEngine.Events;

public class LucidityController : MonoBehaviour
{
    [SerializeField] private float maxLucidity = 100f;
    [SerializeField] private float currentLucidity = 100f;

    public float MaxLucidity => maxLucidity;

    public float CurrentLucidity => currentLucidity;

    public UnityEvent<float> OnLucidityChanged;
    public UnityEvent OnLucidityLost;

    public void AddLucidity(float amount)
    {
        currentLucidity += amount;
        currentLucidity = Mathf.Clamp(currentLucidity, 0, maxLucidity);

        OnLucidityChanged?.Invoke(currentLucidity);

        CheckState();
    }

    public void RemoveLucidity(float amount)
    {
        currentLucidity -= amount;
        currentLucidity = Mathf.Clamp(currentLucidity, 0, maxLucidity);

        OnLucidityChanged?.Invoke(currentLucidity);

        CheckState();
    }

    private void CheckState()
    {
        if (currentLucidity <= 0)
        {
            OnLucidityLost?.Invoke();
            GameManager.Instance.EndGame(false);
        }
    }
}