using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text WaveText;
    [SerializeField] private TMP_Text LifeText;

    private void OnEnable()
    {
        Spawner.OnWaveChanged += UpdateWaveText;
        GameManager.OnLifesChanged += UpdateLifeText;
    }

    private void OnDisable()
    {
        Spawner.OnWaveChanged -= UpdateWaveText;
        GameManager.OnLifesChanged -= UpdateLifeText;
    }

    private void UpdateWaveText(int currentWave)
    {
        WaveText.text = $"Wave: {currentWave + 1}";
    }
    private void UpdateLifeText(int currentLifes)
    {
        LifeText.text = $"Life: {currentLifes}";
    }
}
