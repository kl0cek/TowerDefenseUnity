using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public string levelName; //match a scene name
    public int totalWaves;
    public int startingResources;
    public int StartingHealth;
}
