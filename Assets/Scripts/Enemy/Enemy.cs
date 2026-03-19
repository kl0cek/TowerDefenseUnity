using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Path currentPath;
    private Vector3 _targetPositions;
    private int _currentWaypoint;

    private void Awake()
    {
        currentPath = GameObject.FindGameObjectWithTag("Path").GetComponent<Path>();
    }

    private void OnEnable()
    {
        _targetPositions = currentPath.GetWaypointPosition(1);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPositions, moveSpeed * Time.deltaTime);

        float relativeDistance = (transform.position - _targetPositions).magnitude;
        if (relativeDistance < 0.1f)
        {
            if (_currentWaypoint < currentPath.Waypoints.Length - 1)
            {
                _currentWaypoint++;
                _targetPositions = currentPath.GetWaypointPosition(_currentWaypoint);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
