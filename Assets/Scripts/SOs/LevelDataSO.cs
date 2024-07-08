using UnityEngine;

[CreateAssetMenu(menuName = "Data/Level Data")]

public class LevelDataSO : ScriptableObject
{
    public GameManager.Level LevelID;
    public GameObject BackgroundPrefab;
    public Vector3 BackgroundPos;
    public GameObject BorderPrefab;
    public Vector3 BorderPos;
    public GameObject PlayerPrefab;
    public Vector3 PlayerStartPos;
    public GameObject[] NpcsPrefabs;
    public Vector3[] NpcsStarPos;
    public GameObject[] ObstaclesPrefabs;
    public Vector3[] ObstaclesStartPos;
}
