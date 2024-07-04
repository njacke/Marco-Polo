using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public static Action OnGameStarted;
    public static Action OnGamePaused;
    public static Action<GameOverType> OnGameOver;
    public static Action<NPC.NPCType> OnGuess;

    [SerializeField] private float _guessUIDisplayDelay = 2f;
    [SerializeField] private float _backgroundImage;
    [SerializeField] private LevelDataSO[] _levelsData;

    private Level _currentLevel;
    private GameObject _currentPlayer;
    private bool _firstLevelLoaded = false; // TODO: remove when done testing

    private readonly string SCENE_GAME_STRING = "Game";

    public GameObject GetCurrentPlayer { get => _currentPlayer; }

    public enum Level {
        None,
        Tutorial,
        One,
        Two,
        Three
    }

    public enum GameOverType {
        None,
        Timer,
        Cheat,
        Bite
    }

    public NPC CurrentCaughtNPC { get; private set; } = null;

    private void Start() {
        if (!_firstLevelLoaded) {
            _firstLevelLoaded = true;
            LoadLevel(Level.Tutorial);
        }
    }

    private void OnEnable() {
        NPC.OnCaughtNPC += NPC_OnNPCCaught;
        NPC.OnCheatDetected += NPC_OnCheatDetected;
        GuessUI.OnGuessAnswer += GuessUI_OnGuessAnswer;
        GameOverUI.OnRetry += GameOverUI_OnRetry;
    }

    private void OnDisable() {
        NPC.OnCaughtNPC -= NPC_OnNPCCaught;        
        NPC.OnCheatDetected -= NPC_OnCheatDetected;
        GuessUI.OnGuessAnswer -= GuessUI_OnGuessAnswer;
        GameOverUI.OnRetry -= GameOverUI_OnRetry;
    }

    private void LoadLevel(Level level) {
        var levelData = _levelsData.FirstOrDefault(data => data.LevelID == level);
        if (levelData == null) {
            Debug.Log("No LevelDataSO found for level: " + level.ToString());
            return;
        }
        if (levelData.NpcsPrefabs.Length != levelData.NpcsStarPos.Length) {
            Debug.Log("NPC prefabs and pos length does not match.");
            return;
        }

        // load scene
        //SceneManager.LoadScene(SCENE_GAME_STRING);

        // spawn player
        _currentPlayer = Instantiate(levelData.PlayerPrefab, levelData.PlayerStartPos, Quaternion.identity);

        // spawn NPCs
        for (int i = 0; i < levelData.NpcsPrefabs.Length; i++) {
            Instantiate(levelData.NpcsPrefabs[i], levelData.NpcsStarPos[i], Quaternion.identity);
        }

        _currentLevel = level;
        Time.timeScale = 1f;
        OnGameStarted?.Invoke();
        Debug.Log("Level: " + level.ToString() + " loaded successfully.");
    }

    private void LoadGameOver(GameOverType gameOverType) {
        Time.timeScale = 0f;
        switch (gameOverType) {
            case GameOverType.Cheat:
                break;
            default:
                break;
        }

        OnGameOver?.Invoke(gameOverType);
    }

    private void NPC_OnCheatDetected(NPC sender) {
        LoadGameOver(GameOverType.Cheat);
    }

    private void NPC_OnNPCCaught(NPC sender) {
        StartCoroutine(HandleNPCCaughtRoutine(sender));
    }

    private IEnumerator HandleNPCCaughtRoutine(NPC npc) {
        Time.timeScale = 0;
        OnGamePaused?.Invoke();

        CurrentCaughtNPC = npc;
        
        yield return new WaitForSecondsRealtime(_guessUIDisplayDelay);
        OnGuess?.Invoke(npc.GetNPCType);        
    }

    private void GuessUI_OnGuessAnswer(bool isCorrectAnswer) {
        if (isCorrectAnswer) {
            CurrentCaughtNPC.gameObject.SetActive(false);
        }

        Time.timeScale = 1f;
        OnGameStarted?.Invoke();
    }

    private void GameOverUI_OnRetry() {
        LoadLevel(_currentLevel);      
    }
}
