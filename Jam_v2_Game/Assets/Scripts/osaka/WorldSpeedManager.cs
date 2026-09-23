using System.Collections;
using UnityEngine;

public class WorldSpeedManager : MonoBehaviour
{
    public static WorldSpeedManager Instance { get; private set; }

    [Header("Semáforo Settings")]
    [SerializeField] private float minGreenLightTime = 12f;
    [SerializeField] private float maxGreenLightTime = 25f;
    [SerializeField] private float redLightDuration = 5f;

    public float CurrentSpeed { get; private set; }
    public bool IsRedLight { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(TrafficLightRoutine());
    }

    public void SetWorldSpeed(float speed)
    {
        CurrentSpeed = speed;
    }

    private IEnumerator TrafficLightRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minGreenLightTime, maxGreenLightTime);
            yield return new WaitForSeconds(waitTime);

            // Cambiar a Semáforo en Rojo
            IsRedLight = true;
            Debug.Log("<color=red>¡SEMÁFORO EN ROJO!</color>");

            yield return new WaitForSeconds(redLightDuration);

            // Cambiar a Verde
            IsRedLight = false;
            Debug.Log("<color=green>¡SEMÁFORO EN VERDE!</color>");
        }
    }
}
