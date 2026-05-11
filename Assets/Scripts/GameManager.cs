using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event Action<int> OnLifesChanged;
    public static event Action<int> OnResourcesChanged;
    private int _lifes = 10;
    private int _resources = 175;
    public int Resources => _resources;
    private float _gameSpeed = 1f;
    public float GameSpeed => _gameSpeed;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
        Enemy.OnEnemyRemoved += HandleEnemyRemoved;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
        Enemy.OnEnemyRemoved -= HandleEnemyRemoved;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        OnLifesChanged?.Invoke(_lifes);
        OnResourcesChanged?.Invoke(_resources);
    }

    private void HandleEnemyReachedEnd(EnemyData data)
    {
        _lifes = Mathf.Max(0, _lifes - data.damage);
        OnLifesChanged?.Invoke(_lifes);
    }

    private void HandleEnemyRemoved(Enemy enemy)
    {
        AddResources(Mathf.RoundToInt(enemy.Data.reward));
    }
    private void AddResources(int amount)
    {
        _resources += amount;
        OnResourcesChanged?.Invoke(_resources);
    }
    public void SetTimeScale(float scale) // This method is allowing for pausing and changing game speed in tower menu
    {
        Time.timeScale = scale;
    }
    public void SetGameSpeed(float speed) // This method is used to set the game speed, which can be used to speed up or slow down the game
    {
        _gameSpeed = speed;
        SetTimeScale(_gameSpeed);
    }

    public void SpendResources(int amount)
    {
        if (_resources >= amount)
        {
            _resources -= amount;
            OnResourcesChanged?.Invoke(_resources);
        }
    }

    public void ResetGame()
    {
        _lifes = LevelManagment.Instance.currentLevelData.StartingHealth;
        OnLifesChanged?.Invoke(_lifes);

        _resources = LevelManagment.Instance.currentLevelData.startingResources;
        OnResourcesChanged?.Invoke(_resources);

        SetGameSpeed(1f);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (LevelManagment.Instance != null && LevelManagment.Instance.currentLevelData != null)
        {
            ResetGame();
        }
    }
}
