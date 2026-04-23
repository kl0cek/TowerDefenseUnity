using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static event Action<int> OnLifesChanged;
    public static event Action<int> OnResourcesChanged;
    private int _lifes = 10;
    private int _resources = 0;
    private void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
        Enemy.OnEnemyRemoved += HandleEnemyRemoved;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
        Enemy.OnEnemyRemoved -= HandleEnemyRemoved;
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
}
