using UnityEngine;
using UnityEditor;

public class Path : MonoBehaviour
{
    public GameObject[] Waypoints;

    public Vector3 GetWaypointPosition(int index)
    {
        return Waypoints[index].transform.position;
    }

    private void OnDrawGizmos()
    {
        if (Waypoints.Length > 0)
        {
            for (int i = 0; i < Waypoints.Length; i++)
            {
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.white;
                style.alignment = TextAnchor.MiddleCenter;
                Handles.Label(Waypoints[i].transform.position + Vector3.up * 0.7f, Waypoints[i].name, style);

                if (i < Waypoints.Length - 1)
                {
                    Gizmos.color = Color.gray;
                    Gizmos.DrawLine(Waypoints[i].transform.position, Waypoints[i + 1].transform.position);
                }
            }
            return;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
