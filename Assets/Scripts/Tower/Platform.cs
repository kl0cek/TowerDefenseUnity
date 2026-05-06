using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class Platform : MonoBehaviour
{
    public static event System.Action<Platform> OnPlatformClicked;
    [SerializeField] private LayerMask platformLayerMask;
    public static bool towerPanelOpen { get; set; } = false;
    private void Update()
    {
        if (towerPanelOpen || Time.timeScale == 0f) return;
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D raycastHit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, platformLayerMask);

            if (raycastHit.collider != null)
            {
                Platform platform = raycastHit.collider.GetComponent<Platform>();
                if (platform != null)
                {
                    OnPlatformClicked?.Invoke(platform);
                }
            }
        }
    }

    public void PlaceTower(GameObject towerPrefab)
    {
        Instantiate(towerPrefab, transform.position, Quaternion.identity, transform);
    }
}
