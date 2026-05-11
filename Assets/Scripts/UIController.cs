using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text WaveText;
    [SerializeField] private TMP_Text LifeText;
    [SerializeField] private TMP_Text ResourceText;
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private GameObject towerPanel;
    [SerializeField] private GameObject towerCardsPrefabs;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private TowerData[] towerDatas;
    private List<GameObject> _towerCards = new List<GameObject>();
    private Platform _selectedPlatform;
    [SerializeField] private Button speedOneButton;
    [SerializeField] private Button speedTwoButton;
    [SerializeField] private Button speedThreeButton;
    [SerializeField] private Color normalButtonColor = Color.white;
    [SerializeField] private Color selectedButtonColor = Color.green;
    [SerializeField] private Color normalTextColor = Color.black;
    [SerializeField] private Color selectedTextColor = Color.white;
    [SerializeField] private GameObject pausePanel;
    private bool _isPaused = false;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private GameObject missionCompletePanel;
    private void OnEnable()
    {
        Spawner.OnWaveChanged += UpdateWaveText;
        GameManager.OnLifesChanged += UpdateLifeText;
        GameManager.OnResourcesChanged += UpdateResourceText;
        Platform.OnPlatformClicked += HandlePlatformClick;
        TowerCard.OnTowerCardSelected += HandleTowerSelected;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Spawner.OnMissionComplete += ShowMissionComplete;
    }

    private void OnDisable()
    {
        Spawner.OnWaveChanged -= UpdateWaveText;
        GameManager.OnLifesChanged -= UpdateLifeText;
        GameManager.OnResourcesChanged -= UpdateResourceText;
        Platform.OnPlatformClicked -= HandlePlatformClick;
        TowerCard.OnTowerCardSelected -= HandleTowerSelected;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Spawner.OnMissionComplete -= ShowMissionComplete;
    }

    private void Start()
    {
        speedOneButton.onClick.AddListener(() => SetGameSpeed(0.5f));
        speedTwoButton.onClick.AddListener(() => SetGameSpeed(1f));
        speedThreeButton.onClick.AddListener(() => SetGameSpeed(2f));

        HighlightSelectedSpeedButton(GameManager.Instance.GameSpeed);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    private void UpdateWaveText(int currentWave)
    {
        WaveText.text = $"Wave: {currentWave + 1}";
    }
    private void UpdateLifeText(int currentLifes)
    {
        LifeText.text = $"Life: {currentLifes}";

        if (currentLifes <= 0)
        {
            ShowGameOver();
        }
    }
    private void UpdateResourceText(int currentResources)
    {
        ResourceText.text = $"Resources: {currentResources}";
    }

    private void HandlePlatformClick(Platform platform)
    {
        _selectedPlatform = platform;
        ShowTowerPanel();
    }

    private void ShowTowerPanel()
    {
        towerPanel.SetActive(true);
        Platform.towerPanelOpen = true;
        GameManager.Instance.SetTimeScale(0f);
        PopulateTowerCards();
    }

    public void HideTowerPanel()
    {
        towerPanel.SetActive(false);
        Platform.towerPanelOpen = false;
        GameManager.Instance.SetTimeScale(GameManager.Instance.GameSpeed);
    }

    private void PopulateTowerCards()
    {
        foreach (var card in _towerCards)
        {
            Destroy(card);
        }
        _towerCards.Clear();

        foreach (var towerData in towerDatas)
        {
            GameObject cardObj = Instantiate(towerCardsPrefabs, cardsContainer);
            TowerCard card = cardObj.GetComponent<TowerCard>();
            card.Initilize(towerData);
            _towerCards.Add(cardObj);
        }
    }

    private void HandleTowerSelected(TowerData towerData)
    {
        if (_selectedPlatform.transform.childCount > 0)
        {
            HideTowerPanel();
            StartCoroutine(ShowWarningMessage("This platform is already occupied!"));
            return;
        }

        if (GameManager.Instance.Resources >= towerData.cost)
        {
            GameManager.Instance.SpendResources(towerData.cost);
            _selectedPlatform.PlaceTower(towerData.prefab);
        }
        else
        {
            StartCoroutine(ShowWarningMessage("Not enough resources!"));
        }
        HideTowerPanel();
    }

    private IEnumerator ShowWarningMessage(string message)
    {
        warningText.text = message;
        warningText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        warningText.gameObject.SetActive(false);
    }

    private void SetGameSpeed(float speed)
    {
        HighlightSelectedSpeedButton(speed);
        GameManager.Instance.SetGameSpeed(speed);
    }

    private void UpdateSpeedButtonColors(Button button, bool isSelected)
    {
        button.image.color = isSelected ? selectedButtonColor : normalButtonColor;

        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        if (buttonText != null)
        {
            buttonText.color = isSelected ? selectedTextColor : normalTextColor;
        }
    }
    private void HighlightSelectedSpeedButton(float selectedSpeed)
    {
        UpdateSpeedButtonColors(speedOneButton, selectedSpeed == 0.5f);
        UpdateSpeedButtonColors(speedTwoButton, selectedSpeed == 1f);
        UpdateSpeedButtonColors(speedThreeButton, selectedSpeed == 2f);
    }

    public void TogglePause()
    {
        if (towerPanel.activeSelf) return;
        if (_isPaused)
        {
            pausePanel.SetActive(false);
            _isPaused = false;
            GameManager.Instance.SetTimeScale(GameManager.Instance.GameSpeed);
        }
        else
        {
            pausePanel.SetActive(true);
            _isPaused = true;
            GameManager.Instance.SetTimeScale(0f);
        }
    }

    public void RestartGame()
    {
        GameManager.Instance.SetTimeScale(1f);
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void MainMenu()
    {
        GameManager.Instance.SetTimeScale(1f);
        SceneManager.LoadScene("MainMenu");
    }

    private void ShowGameOver()
    {
        GameManager.Instance.SetTimeScale(0f);
        gameOverPanel.SetActive(true);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(ShowObjectiveMessage("Defend your base from incoming waves of enemies!"));
    }

    private IEnumerator ShowObjectiveMessage(string message)
    {
        objectiveText.text = $"Survive {LevelManagment.Instance.currentLevelData.totalWaves} waves of enemies!";
        objectiveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(4f);
        objectiveText.gameObject.SetActive(false);
    }

    private void ShowMissionComplete()
    {
        missionCompletePanel.SetActive(true);
        GameManager.Instance.SetTimeScale(0f);
    }

    public void EnterEndlessMode()
    {
        missionCompletePanel.SetActive(false);
        GameManager.Instance.SetTimeScale(GameManager.Instance.GameSpeed);
        Spawner.Instance.EnableEndlessMode();
    }
}
