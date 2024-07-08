using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static Action OnGameStarted;
    public static Action OnGamePaused;
    public static Action<GameOverType> OnGameOver;
    public static Action OnLevelCompleted;
    public static Action<NPC.NPCType> OnGuess;
    public static Action<Level> OnLevelLoaded;
    public static Action OnContestantEliminated;

    [SerializeField] private float _guessUIDisplayDelay = 2f;
    [SerializeField] private float _gameOverUIDisplayDelay = 2f;
    [SerializeField] private float _backgroundImage;
    [SerializeField] private LevelDataSO[] _levelsData;

    private Level _activeLevel;
    private GameObject _activePlayer;
    private NPC[] _activeNPCs;
    private NPC.NPCType[] _startingNPCTypes;
    private int _contestantsCount = 0;
    private int _contestantsEliminated = 0;

    private Coroutine _npcCaughtRoutine;
    private bool _firstLevelLoaded = false; // TODO: remove when done testing

    public NPC CurrentCaughtNPC { get; private set; } = null;
    public GameObject GetActivePlayer { get => _activePlayer; }
    public Level GetActiveLevel { get => _activeLevel; }
    public NPC[] GetActiveNPCs { get => _activeNPCs; }
    public NPC.NPCType[] GetStartingNPCTypes { get => _startingNPCTypes; }

    public enum Level {
        None,
        Tutorial,
        Pool,
        Garden,
        Terrace,
        Lobby
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
            LoadLevel(Level.Pool);
        }
    }

    private void OnEnable() {
        Blindfold.OnBlinfoldReady += Blindfold_OnBlindfoldReady;
        OnGameStarted += GameManager_OnGameStarted;
        NPC.OnCaughtNPC += NPC_OnNPCCaught;
        NPC.OnCheatDetected += NPC_OnCheatDetected;
        TimerUI.OnTimerStep += TimerUI_OnTimerStep;
        GuessUI.OnGuessEnd += GuessUI_OnGuessEnd;
        GameOverUI.OnRetry += GameOverUI_OnRetry;
        CompanionUI.OnContinue += CompanionUI_OnContinue;
    }


    private void OnDisable() {
        Blindfold.OnBlinfoldReady -= Blindfold_OnBlindfoldReady;
        OnGameStarted -= GameManager_OnGameStarted;
        NPC.OnCaughtNPC -= NPC_OnNPCCaught;        
        NPC.OnCheatDetected -= NPC_OnCheatDetected;
        TimerUI.OnTimerStep -= TimerUI_OnTimerStep;
        GuessUI.OnGuessEnd -= GuessUI_OnGuessEnd;
        GameOverUI.OnRetry -= GameOverUI_OnRetry;
        CompanionUI.OnContinue -= CompanionUI_OnContinue;
    }

    public async void LoadLevel(Level level) {
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
        await SceneLoader.LoadSceneAsync(SceneLoader.SCENE_GAME_STRING);
        _activeLevel = level;

        // instantiate background
        Instantiate(levelData.BackgroundPrefab, levelData.BackgroundPos, Quaternion.identity);

        // instantiate border
        Instantiate(levelData.BorderPrefab, levelData.BorderPos, Quaternion.identity);

        // instantiate player
        _activePlayer = Instantiate(levelData.PlayerPrefab, levelData.PlayerStartPos, Quaternion.identity);

        // instantiate NPCs
        _activeNPCs = new NPC[levelData.NpcsPrefabs.Length];
        _startingNPCTypes = new NPC.NPCType[levelData.NpcsPrefabs.Length];

        for (int i = 0; i < levelData.NpcsPrefabs.Length; i++) {
            var newNPC = Instantiate(levelData.NpcsPrefabs[i], levelData.NpcsStarPos[i], Quaternion.identity).GetComponent<NPC>();
            _activeNPCs[i] = newNPC;
            _startingNPCTypes[i] = newNPC.GetNPCType;
            if (newNPC.IsContestant) _contestantsCount++;
        }

        // instantiate obstacles
        for (int i = 0; i < levelData.ObstaclesPrefabs.Length; i++) {
            Instantiate(levelData.ObstaclesPrefabs[i], levelData.ObstaclesStartPos[i], Quaternion.identity);
        }

        OnLevelLoaded?.Invoke(_activeLevel);
        Time.timeScale = 1f;

        Debug.Log("Level: " + level.ToString() + " loaded successfully.");
    }

    private IEnumerator LoadGameOver(GameOverType gameOverType) {
        Debug.Log("Game over called.");
        Time.timeScale = 0f;
        OnGamePaused?.Invoke();
        yield return new WaitForSecondsRealtime(_gameOverUIDisplayDelay);
        OnGameOver?.Invoke(gameOverType);
    }

    private void Blindfold_OnBlindfoldReady() {
        OnGameStarted?.Invoke();
    }

    private void GameManager_OnGameStarted() {
        // set to defualt positions to avoid trapping NPCs after wrong guess
        var levelData = _levelsData.FirstOrDefault(data => data.LevelID == _activeLevel);
        for (int i = 0; i < _activeNPCs.Length; i++) {
            if (_activeNPCs[i] != null) {
                NPC npc = _activeNPCs[i].GetComponent<NPC>();
                npc.transform.position = levelData.NpcsStarPos[i];
            }
        }
    }

    private void NPC_OnCheatDetected(NPC sender, bool waveTriggered) {
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
            OnContestantEliminated?.Invoke();
        }

        Time.timeScale = 1f;

        if (_contestantsEliminated >= _contestantsCount) {
            OnLevelCompleted?.Invoke();
        } else {
            OnGameStarted?.Invoke();
        }
    }

    private void GameOverUI_OnRetry() {
        LoadLevel(_activeLevel);      
    }

    private void CompanionUI_OnContinue() {
        if (_activeLevel == Level.Pool) {
            LoadLevel(Level.Garden);
        }
        if (_activeLevel  == Level.Garden) {
            LoadLevel(Level.Terrace);
        }
        if (_activeLevel == Level.Terrace) {
            LoadLevel(Level.Lobby);
        }
        if (_activeLevel == Level.Lobby) {
            SceneLoader.LoadScene(SceneLoader.SCENE_END_MENU_STRING);
        }
    }
}
