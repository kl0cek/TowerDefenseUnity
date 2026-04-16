using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static event Action<int> OnLifesChanged;
    private int _lifes = 4;
    private void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
    }

    private void Start()
    {
        OnLifesChanged?.Invoke(_lifes);
    }

    private void HandleEnemyReachedEnd(EnemyData data)
    {
        _lifes = Mathf.Max(0, _lifes - data.damage);
        OnLifesChanged?.Invoke(_lifes);
    }
}
