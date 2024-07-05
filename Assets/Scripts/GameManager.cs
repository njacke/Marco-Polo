using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static Action OnGameStarted;
    public static Action OnGamePaused;
    public static Action<GameOverType> OnGameOver;
    public static Action OnLevelCompleted;
    public static Action<NPC.NPCType> OnGuess;
    public static Action OnGameObjectsInstantiated;

    [SerializeField] private float _guessUIDisplayDelay = 2f;
    [SerializeField] private float _cheatGameOverDelay = 1f;
    [SerializeField] private float _gameOverUIDisplayDelay = 2f;
    [SerializeField] private float _backgroundImage;
    [SerializeField] private LevelDataSO[] _levelsData;

    private Level _currentLevel;
    private GameObject _currentPlayer;
    private NPC[] _currentNPCs;
    private int _contestantsCount = 0;
    private int _contestantsEliminated = 0;

    private Coroutine _npcCaughtRoutine;
    private bool _firstLevelLoaded = false; // TODO: remove when done testing

    private readonly string SCENE_GAME_STRING = "Game";

    public NPC CurrentCaughtNPC { get; private set; } = null;
    public GameObject GetCurrentPlayer { get => _currentPlayer; }
    public NPC[] GetCurrentNPCs { get => _currentNPCs; }
    public float GetCheatGameOverDelay { get => _cheatGameOverDelay; }

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


    private void Start() {
        if (!_firstLevelLoaded) {
            _firstLevelLoaded = true;
            LoadLevel(Level.Tutorial);
        }
    }

    private void OnEnable() {
        OnGameStarted += GameManager_OnGameStarted;
        NPC.OnCaughtNPC += NPC_OnNPCCaught;
        NPC.OnCheatDetected += NPC_OnCheatDetected;
        TimerUI.OnTimerStep += TimerUI_OnTimerStep;
        GuessUI.OnGuessEnd += GuessUI_OnGuessEnd;
        GameOverUI.OnRetry += GameOverUI_OnRetry;
        WinScreenUI.OnContinue += WinScreenUI_OnContinue;
    }


    private void OnDisable() {
        OnGameStarted -= GameManager_OnGameStarted;
        NPC.OnCaughtNPC -= NPC_OnNPCCaught;        
        NPC.OnCheatDetected -= NPC_OnCheatDetected;
        TimerUI.OnTimerStep -= TimerUI_OnTimerStep;
        GuessUI.OnGuessEnd -= GuessUI_OnGuessEnd;
        GameOverUI.OnRetry -= GameOverUI_OnRetry;
        WinScreenUI.OnContinue -= WinScreenUI_OnContinue;
    }

    private async void LoadLevel(Level level) {
        _contestantsCount = 0;
        _contestantsEliminated = 0;
        var levelData = _levelsData.FirstOrDefault(data => data.LevelID == level);
        if (levelData == null) {
            Debug.Log("No LevelDataSO found for level: " + level.ToString());
            return;
        }
        if (levelData.NpcsPrefabs.Length != levelData.NpcsStarPos.Length) {
            Debug.Log("NPC prefabs and pos length does not match.");
            return;
        }

        // load scene async before instantiating objects
        await SceneLoader.LoadSceneAsync(SCENE_GAME_STRING);
        _currentLevel = level;

        // spawn player
        _currentPlayer = Instantiate(levelData.PlayerPrefab, levelData.PlayerStartPos, Quaternion.identity);

        // spawn NPCs
        _currentNPCs = new NPC[levelData.NpcsPrefabs.Length];

        for (int i = 0; i < levelData.NpcsPrefabs.Length; i++) {
            var newNPC = Instantiate(levelData.NpcsPrefabs[i], levelData.NpcsStarPos[i], Quaternion.identity).GetComponent<NPC>();
            _currentNPCs[i] = newNPC;
            if (newNPC.IsContestant) _contestantsCount++;
        }

        OnGameObjectsInstantiated?.Invoke();
        Time.timeScale = 1f;
        OnGameStarted?.Invoke();

        Debug.Log("Level: " + level.ToString() + " loaded successfully.");
    }

    private IEnumerator LoadGameOver(GameOverType gameOverType) {
        Time.timeScale = 0f;
        OnGamePaused?.Invoke();
        yield return new WaitForSecondsRealtime(_gameOverUIDisplayDelay);
        OnGameOver?.Invoke(gameOverType);
    }

    private void GameManager_OnGameStarted() {
        // set to defualt positions to avoid trapping NPCs after wrong guess
        var levelData = _levelsData.FirstOrDefault(data => data.LevelID == _currentLevel);
        for (int i = 0; i < _currentNPCs.Length; i++) {
            if (_currentNPCs[i] != null) {
                NPC npc = _currentNPCs[i].GetComponent<NPC>();
                npc.transform.position = levelData.NpcsStarPos[i];
            }
        }
    }

    private void NPC_OnCheatDetected(NPC sender) {
        StartCoroutine(OnCheatDetectedRoutine(sender));
    }

    private IEnumerator OnCheatDetectedRoutine(NPC sender) {
        OnGamePaused?.Invoke();
        yield return sender.DisplayCheatVisual();
        StartCoroutine(LoadGameOver(GameOverType.Cheat));
    }

    private void NPC_OnNPCCaught(NPC sender) {
        // game over > guess priority + only one routine at a time
        if (_npcCaughtRoutine == null) {
            _npcCaughtRoutine = StartCoroutine(HandleNPCCaughtRoutine(sender));
        } else if (sender.GetNPCType == NPC.NPCType.Dog) {
            StopCoroutine(_npcCaughtRoutine);
            _npcCaughtRoutine = StartCoroutine(HandleNPCCaughtRoutine(sender));
        }
    }

    private IEnumerator HandleNPCCaughtRoutine(NPC npc) {
        Time.timeScale = 0;
        OnGamePaused?.Invoke();

        CurrentCaughtNPC = npc;
        
        if (npc.GetNPCType == NPC.NPCType.Dog) {
            StartCoroutine(LoadGameOver(GameOverType.Bite));
        } else {
            yield return new WaitForSecondsRealtime(_guessUIDisplayDelay);
            OnGuess?.Invoke(npc.GetNPCType);
        }

        _npcCaughtRoutine = null;
    }

    private void TimerUI_OnTimerStep(bool isFinalStep, int stepIndex) {
        if (isFinalStep) {
            Time.timeScale = 0;
            OnGamePaused?.Invoke();
            OnGameOver?.Invoke(GameOverType.Timer);
        }
    }

    private void GuessUI_OnGuessEnd(bool isCorrectAnswer) {
        if (isCorrectAnswer) {
            CurrentCaughtNPC.gameObject.SetActive(false);
            _contestantsEliminated++;
        }

        Time.timeScale = 1f;

        if (_contestantsEliminated >= _contestantsCount) {
            OnLevelCompleted?.Invoke();
        } else {
            OnGameStarted?.Invoke();
        }
    }

    private void GameOverUI_OnRetry() {
        LoadLevel(_currentLevel);      
    }

    private void WinScreenUI_OnContinue() {
        // TODO: load next level/cinematic/whatever
    }
}
