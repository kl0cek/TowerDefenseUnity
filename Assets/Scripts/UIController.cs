using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text WaveText;
    [SerializeField] private TMP_Text LifeText;
    [SerializeField] private TMP_Text ResourceText;
    [SerializeField] private GameObject towerPanel;
    [SerializeField] private GameObject towerCardsPrefabs;
    [SerializeField] private Transform cardsContainer;

    [SerializeField] private TowerData[] towerDatas;
    private List<GameObject> _towerCards = new List<GameObject>();

    private Platform _selectedPlatform;

    private void OnEnable()
    {
        Spawner.OnWaveChanged += UpdateWaveText;
        GameManager.OnLifesChanged += UpdateLifeText;
        GameManager.OnResourcesChanged += UpdateResourceText;
        Platform.OnPlatformClicked += HandlePlatformClick;
        TowerCard.OnTowerCardSelected += HandleTowerSelected;
    }

    private void OnDisable()
    {
        Spawner.OnWaveChanged -= UpdateWaveText;
        GameManager.OnLifesChanged -= UpdateLifeText;
        GameManager.OnResourcesChanged -= UpdateResourceText;
        Platform.OnPlatformClicked -= HandlePlatformClick;
        TowerCard.OnTowerCardSelected -= HandleTowerSelected;
    }

    private void UpdateWaveText(int currentWave)
    {
        WaveText.text = $"Wave: {currentWave + 1}";
    }
    private void UpdateLifeText(int currentLifes)
    {
        LifeText.text = $"Life: {currentLifes}";
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
        GameManager.Instance.SetTimeScale(0f);
        PopulateTowerCards();
    }

    public void HideTowerPanel()
    {
        towerPanel.SetActive(false);
        GameManager.Instance.SetTimeScale(1f);
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
        _selectedPlatform.PlaceTower(towerData.prefab);
        HideTowerPanel();
    }
}
