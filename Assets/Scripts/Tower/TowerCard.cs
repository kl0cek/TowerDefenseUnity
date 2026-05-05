using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TowerCard : MonoBehaviour
{
    [SerializeField] private Image towerImage;
    [SerializeField] private TMP_Text costText;
    private TowerData _towerData;
    public static event System.Action<TowerData> OnTowerCardSelected;

    public void Initilize(TowerData data)
    {
        _towerData = data;
        towerImage.sprite = data.sprite;
        costText.text = $"Cost: {data.cost}";
    }

    public void PlaceTower()
    {
        OnTowerCardSelected?.Invoke(_towerData);
    }
}
