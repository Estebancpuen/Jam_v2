using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private float journeyDuration = 180f;

    private float currentTime;

    private void Start()
    {
        currentTime = 0f;
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing)
            return;

        currentTime += Time.deltaTime;

        if (currentTime >= journeyDuration)
        {
            FinishJourney();
        }
    }

    private void FinishJourney()
    {
        GameManager.Instance.EndGame(true);
    }
}