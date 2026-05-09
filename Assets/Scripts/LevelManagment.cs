using UnityEngine;

public class LevelManagment : MonoBehaviour
{
    public static LevelManagment Instance { get; private set; }
    public LevelData[] levels;
    public LevelData currentLevelData { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
        currentLevelData = levels[0];
    }
}
