using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    public static event Action<EnemyData> OnEnemyReachedEnd;
    public static event System.Action<Enemy> OnEnemyRemoved;
    private Path _currentPath;
    private Vector3 _targetPositions;
    private int _currentWaypoint;

    private void Awake()
    {
        _currentPath = GameObject.FindGameObjectWithTag("Path").GetComponent<Path>();
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
}
