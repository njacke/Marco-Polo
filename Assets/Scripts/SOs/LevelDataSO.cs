using UnityEngine;

[CreateAssetMenu(menuName = "Data/Level Data")]

public class LevelDataSO : ScriptableObject
{
    public GameManager.Level LevelID;
    public Sprite BackgroundSprite;
    public GameObject PlayerPrefab;
    public Vector3 PlayerStartPos;
    public GameObject[] NpcsPrefabs;
    public Vector3[] NpcsStarPos;
}
