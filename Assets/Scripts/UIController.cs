using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text WaveText;
    [SerializeField] private TMP_Text LifeText;
    [SerializeField] private TMP_Text ResourceText;
    [SerializeField] private GameObject towerPanel;

    private void OnEnable()
    {
        Spawner.OnWaveChanged += UpdateWaveText;
        GameManager.OnLifesChanged += UpdateLifeText;
        GameManager.OnResourcesChanged += UpdateResourceText;
        Platform.OnPlatformClicked += HandlePlatformClick;
    }

    private void OnDisable()
    {
        Spawner.OnWaveChanged -= UpdateWaveText;
        GameManager.OnLifesChanged -= UpdateLifeText;
        GameManager.OnResourcesChanged -= UpdateResourceText;
        Platform.OnPlatformClicked -= HandlePlatformClick;
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
        ShowTowerPanel();
    }

    private void ShowTowerPanel()
    {
        towerPanel.SetActive(true);
        GameManager.Instance.SetTimeScale(0f);
    }

    public void HideTowerPanel()
    {
        towerPanel.SetActive(false);
        GameManager.Instance.SetTimeScale(1f);
    }
}
