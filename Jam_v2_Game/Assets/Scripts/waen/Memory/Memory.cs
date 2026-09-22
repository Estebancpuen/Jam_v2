using UnityEngine;

public class Memory : MonoBehaviour
{
    [SerializeField] private MemoryData data;

    private bool alreadyInteracted;

    public MemoryData Data => data;

    public void Interact()
    {
        if (alreadyInteracted)
            return;

        alreadyInteracted = true;

        LucidityController lucidity =
            FindFirstObjectByType<LucidityController>();

        if (lucidity == null)
        {
            Debug.LogWarning("No se encontró LucidityController.");
            return;
        }

        switch (data.type)
        {
            case MemoryType.Correct:

                lucidity.AddLucidity(data.lucidityReward);

                Debug.Log("Recuerdo correcto: " + data.memoryName);

                break;

            case MemoryType.False:

                lucidity.RemoveLucidity(data.lucidityPenalty);

                Debug.Log("Recuerdo falso: " + data.memoryName);

                break;

            case MemoryType.Neutral:

                Debug.Log("Elemento neutral: " + data.memoryName);

                break;
        }

        Destroy(gameObject);
    }
}