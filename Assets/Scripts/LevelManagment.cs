using UnityEngine;
using UnityEngine.SceneManagement;

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
            DontDestroyOnLoad(gameObject);
        }
        currentLevelData = levels[0];
    }

    public void LoadLevel(LevelData levelData)
    {
        currentLevelData = levelData;
        SceneManager.LoadScene(levelData.levelName);
    }
}
