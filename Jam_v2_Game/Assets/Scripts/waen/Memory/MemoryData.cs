using UnityEngine;

[CreateAssetMenu(
    fileName = "MemoryData",
    menuName = "Game/Memory Data"
)]
public class MemoryData : ScriptableObject
{
    public string memoryName;

    public MemoryType type;

    public float lucidityReward = 10f;

    public float lucidityPenalty = 10f;

    public AudioClip sound;

    public Sprite memoryImage;
}

public enum MemoryType
{
    Correct,
    False,
    Neutral
}