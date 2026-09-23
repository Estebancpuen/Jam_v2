using UnityEngine;
using UnityEngine.UI;

public class UIcontrollerPrueba : MonoBehaviour
{
    [SerializeField] private Text distanceText;
    private float totalDistance = 0f;

    private void Update()
    {
        if (WorldSpeedManager.Instance != null)
        {
            
            totalDistance += WorldSpeedManager.Instance.CurrentSpeed * Time.deltaTime;

            if (distanceText != null)
            {
                distanceText.text = Mathf.FloorToInt(totalDistance) + " m"; 
            }
        }
    }
}
