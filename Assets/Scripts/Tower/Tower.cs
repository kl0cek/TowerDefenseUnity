using UnityEngine;
using System.Collections.Generic;

public class Tower : MonoBehaviour
{
    [SerializeField] private TowerData data;
    private CircleCollider2D _rangeCollider;

    private List<Enemy> _enemiesInRange;
    private ObjectPooler _projectilePooler;

    private float _shootTimer;

    private void Start()
    {
        _rangeCollider = GetComponent<CircleCollider2D>();
        _rangeCollider.radius = data.range;
        _enemiesInRange = new List<Enemy>();
        _projectilePooler = GetComponent<ObjectPooler>();
        _shootTimer = data.fireRate;
    }

    private void Update()
    {
        _shootTimer -= Time.deltaTime;
        if (_shootTimer <= 0f)
        {
            _shootTimer = data.fireRate;
            Shoot();
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, data.range);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            _enemiesInRange.Add(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            _enemiesInRange.Remove(enemy);
        }
    }

    private void Shoot()
    {
        if (_enemiesInRange.Count > 0)
        {
            GameObject projectileObj = _projectilePooler.GetPooledObject();
            projectileObj.transform.position = transform.position;
            projectileObj.SetActive(true);
            Vector2 _shootDirection = (_enemiesInRange[0].transform.position - transform.position).normalized;
            projectileObj.GetComponent<Projectile>().Shoot(data, _shootDirection);

        }
    }
}
