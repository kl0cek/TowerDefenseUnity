using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    [SerializeField] private WaveData[] wavesData;
    private int _currentWaveIndex = 0;
    private WaveData CurrentWave => wavesData[_currentWaveIndex];
    private float _spawnTimer;
    private int _spawnnedCounter;
    private int _enemiesRemoved;
    [SerializeField] private ObjectPooler goblinPool;
    [SerializeField] private ObjectPooler flyPool;
    [SerializeField] private ObjectPooler riderPool;
    private Dictionary<EnemyType, ObjectPooler> _poolDictionary;

    private float _timeBeetwenWaves = 2f;
    private float _waveCooldown;
    private bool _isWaveCooldown = false;
    private void Awake()
    {
        _poolDictionary = new Dictionary<EnemyType, ObjectPooler>()
        {
            { EnemyType.Goblin, goblinPool },
            { EnemyType.Fly, flyPool },
            { EnemyType.Rider, riderPool }
        };
    }
    private void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
    }
    void Update()
    {
        if (_isWaveCooldown)
        {
            _waveCooldown -= Time.deltaTime;
            if (_waveCooldown <= 0f)
            {
                _currentWaveIndex = (_currentWaveIndex + 1) % wavesData.Length;
                _spawnnedCounter = 0;
                _enemiesRemoved = 0;
                _spawnTimer = CurrentWave.spawnInterval;
                _isWaveCooldown = false;
            }
        }
        else
        {
            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer <= 0 && _spawnnedCounter < CurrentWave.enemyPerWave)
            {
                _spawnTimer = CurrentWave.spawnInterval;
                SpawnEnemy();
                _spawnnedCounter++;
            }
            else if (_spawnnedCounter >= CurrentWave.enemyPerWave && _enemiesRemoved >= CurrentWave.enemyPerWave)
            {
                _isWaveCooldown = true;
                _waveCooldown = _timeBeetwenWaves;
            }
        }
    }

    private void SpawnEnemy()
    {
        if (_poolDictionary.TryGetValue(CurrentWave.enemyType, out ObjectPooler pool))
        {
            GameObject spawnedObject = pool.GetPooledObject();
            spawnedObject.transform.position = transform.position;
            spawnedObject.SetActive(true);
        }
        else
        {
            Debug.LogError($"No pool found for enemy type: {CurrentWave.enemyType}");
        }
    }

    private void HandleEnemyReachedEnd(EnemyData data)
    {
        _enemiesRemoved++;
    }
}
