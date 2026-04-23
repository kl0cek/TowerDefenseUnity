using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    public static event Action<EnemyData> OnEnemyReachedEnd;
    public static event Action<Enemy> OnEnemyRemoved;
    private Path _currentPath;
    private Vector3 _targetPositions;
    private int _currentWaypoint;

    private float _lives;
    private float _maxlives;
    [SerializeField] private Transform _healthBar;
    private Vector3 _healthBarScale;

    private void Awake()
    {
        _currentPath = GameObject.FindGameObjectWithTag("Path").GetComponent<Path>();
        _healthBarScale = _healthBar.localScale;
    }

    private void OnEnable()
    {
        _currentWaypoint = 0;
        _targetPositions = _currentPath.GetWaypointPosition(0);
    }
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPositions, data.speed * Time.deltaTime);

        float relativeDistance = (transform.position - _targetPositions).magnitude;
        if (relativeDistance < 0.1f)
        {
            if (_currentWaypoint < _currentPath.Waypoints.Length - 1)
            {
                _currentWaypoint++;
                _targetPositions = _currentPath.GetWaypointPosition(_currentWaypoint);
            }
            else
            {
                OnEnemyReachedEnd?.Invoke(data);
                gameObject.SetActive(false);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        _lives -= damage;
        _lives = Mathf.Max(0, _lives);
        UpdateHealthBar();
        if (_lives <= 0)
        {
            OnEnemyRemoved?.Invoke(this);
            gameObject.SetActive(false);
        }
    }

    private void UpdateHealthBar()
    {
        float healthPercent = _lives / _maxlives;
        Vector3 newScale = _healthBarScale;
        newScale.x *= healthPercent;
        _healthBar.localScale = newScale;
    }

    public void Initilize(float healthMultiplier)
    {
        _maxlives = data.lives * healthMultiplier;
        _lives = _maxlives;
        UpdateHealthBar();
    }
}
