using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static Action OnReady;
    public static Action OnCatch;
    public static Action OnScan;
    public static Action<float> OnCheat;

    [SerializeField] private float _globalCooldownDuration = 2f;
    [SerializeField] private float _cheatSkillDuration = 1f;
    [SerializeField] private float _baseMoveSpeed = 100f;
    
    private Rigidbody2D _rb;
    private PlayerInput _playerInput;
    private Catch _catch; 
    private SoundWave _soundWave; 
    private GlobalCooldown _globalCooldown;
    private FrameInput _frameInput;

    private bool _isReady = false;
    private bool _controlsLocked = true;

    // TUTORIAL
    private bool _isTutorial = false;
    private bool _movementLocked = true;
    private bool _catchLocked = true;
    private bool _scanLocked = true;
    private bool _cheatLocked = true;
    private bool _readyLocked = true;

    public enum SkillType {
        None,
        Catch,
        Scan,
        Cheat
    }
    
    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _catch = GetComponentInChildren<Catch>();
        _soundWave = GetComponentInChildren<SoundWave>();
        _globalCooldown = new GlobalCooldown(_globalCooldownDuration);
    }

    private void Start() {
        if (TutorialManager.Instance != null) {
            _isTutorial = true;
        }
    }

    private void Update() {
        GatherInput();

        if (!_isReady && !_isTutorial) {
            HandleReady();
        }

        if (!_controlsLocked && !_isTutorial) {
            _globalCooldown.TrackCooldown();
            HandleCatch();
            HandleScan();
            HandleCheat();
        }

        // tutorial

        if (!_isReady && !_readyLocked && _isTutorial) {
            HandleReady();
        }

        if (!_catchLocked && _isTutorial) {
            HandleCatch();
            _globalCooldown.TrackCooldown();

        }
        if (!_scanLocked && _isTutorial) {
            HandleScan();
            _globalCooldown.TrackCooldown();
        }
        if (!_cheatLocked && _isTutorial) {
            HandleCheat();
            _globalCooldown.TrackCooldown();
        }

    }

    private void FixedUpdate() {
        if (!_controlsLocked && !_isTutorial) {
            Move();
        }

        if (!_movementLocked && _isTutorial) {
            Move();
        }
    }

    private void OnEnable() {
        GameManager.OnGameStarted += GameManager_OnGameStarted;
        GameManager.OnGamePaused += GameManager_OnGamePaused;
        CountdownUI.OnCountdownStep += CountdownUI_OnCountdownStep;

        TutorialPopUpsUI.OnMovementSlideDisplayed += TutorialPopupsUI_OnMovementSlideDisplayed;
        TutorialManager.OnMoveTutorialDone += TutorialManager_OnMoveTutorialDone;
        TutorialPopUpsUI.OnBlindfoldSlideDiplayed += OnBlindfoldSlideDiplayed;
        TutorialPopUpsUI.OnScanSlideDisplayed += TutorialManager_OnScanSlideDisplayed;
        TutorialManager.OnScanTutorialDone += TutorialManager_OnScanTutorialDone;
        TutorialPopUpsUI.OnCheatSlideDisplayed += TutorialPopUpsUI_OnCheatSlideDisplayed;
        TutorialPopUpsUI.OnCheatTutorialDone += TutorialPopUpsUI_OnCheatTutorialDone;
        TutorialPopUpsUI.OnFindSlideDisplayed += TutorialPopUpsUI_OnFindSlideDisplayed;
        TutorialManager.OnFindTutorialDone += TutorialManager_OnFindTutorialDone;
        TutorialPopUpsUI.OnCatchSlideDisplayed += TutorialPopUpsUI_OnCatchSlideDisplayed;
        NPC.OnCaughtNPC += NPC_OnNPCCaught;         
    }


    private void OnDisable() {
        GameManager.OnGameStarted -= GameManager_OnGameStarted;
        GameManager.OnGamePaused -= GameManager_OnGamePaused;
        CountdownUI.OnCountdownStep -= CountdownUI_OnCountdownStep;

        TutorialPopUpsUI.OnMovementSlideDisplayed -= TutorialPopupsUI_OnMovementSlideDisplayed;
        TutorialManager.OnMoveTutorialDone -= TutorialManager_OnMoveTutorialDone;
        TutorialPopUpsUI.OnBlindfoldSlideDiplayed -= OnBlindfoldSlideDiplayed;
        TutorialPopUpsUI.OnScanSlideDisplayed -= TutorialManager_OnScanSlideDisplayed;
        TutorialManager.OnScanTutorialDone -= TutorialManager_OnScanTutorialDone;
        TutorialPopUpsUI.OnCheatSlideDisplayed -= TutorialPopUpsUI_OnCheatSlideDisplayed;
        TutorialPopUpsUI.OnCheatTutorialDone += TutorialPopUpsUI_OnCheatTutorialDone;
        TutorialPopUpsUI.OnFindSlideDisplayed -= TutorialPopUpsUI_OnFindSlideDisplayed;
        TutorialManager.OnFindTutorialDone -= TutorialManager_OnFindTutorialDone;
        TutorialPopUpsUI.OnCatchSlideDisplayed -= TutorialPopUpsUI_OnCatchSlideDisplayed;
        NPC.OnCaughtNPC -= NPC_OnNPCCaught;   
    }


    // TUTORIAL

    private void NPC_OnNPCCaught(NPC npc) {
        if (_isTutorial) {
            _catchLocked = true;
        }
    }

    private void TutorialPopUpsUI_OnCatchSlideDisplayed() {
        _catchLocked = false;
        _frameInput = new FrameInput();
        _rb.velocity = Vector2.zero; 
    }

    private void TutorialManager_OnFindTutorialDone() {
        _movementLocked = true;
        _scanLocked = true;
        _cheatLocked = true;
        _frameInput = new FrameInput();
        _rb.velocity = Vector2.zero; 
    }

    private void TutorialPopUpsUI_OnFindSlideDisplayed() {
        _movementLocked = false;
        _scanLocked = false;
        _cheatLocked = false;
        _frameInput = new FrameInput();
        _rb.velocity = Vector2.zero;        
    }

    private void TutorialPopUpsUI_OnCheatTutorialDone() {
        _cheatLocked = true;
        _frameInput = new FrameInput();
        _rb.velocity = Vector2.zero;  
    }

    private void TutorialPopUpsUI_OnCheatSlideDisplayed() {
        _cheatLocked = false;
        _frameInput = new FrameInput();
        _rb.velocity = Vector2.zero;
    }

    private void TutorialManager_OnScanTutorialDone() {
        _movementLocked = true;
        _scanLocked = true;
        _frameInput = new FrameInput();
        _rb.velocity = Vector2.zero;
    }

    private void TutorialManager_OnScanSlideDisplayed() {
        _movementLocked = false;
        _scanLocked = false;
        _frameInput = new FrameInput();
        _rb.velocity = Vector2.zero;

    }

    private void OnBlindfoldSlideDiplayed() {
        _readyLocked = false;
    }

    private void TutorialManager_OnMoveTutorialDone() {
        _movementLocked = true;
        _frameInput = new FrameInput();
        _rb.velocity = Vector2.zero;

    }

    private void TutorialPopupsUI_OnMovementSlideDisplayed() {
        _movementLocked = false;
        _frameInput = new FrameInput();
        _rb.velocity = Vector2.zero;
    }

    // CONTROLLER

    private void GatherInput() {
        _frameInput = _playerInput.FrameInput;
    }

    private void Move() {
        Vector2 movement = _baseMoveSpeed * Time.fixedDeltaTime * _frameInput.Move;
        //Debug.Log("New movement: " + movement);
        _rb.velocity = movement;
    }

    private void HandleReady() {
        if (_frameInput.Cheat) {
            Debug.Log("Player is ready.");
            _isReady = true;
            _soundWave.TriggerSoundWave();
            OnReady?.Invoke();
        }
    }

    private void HandleCatch() {
        if (_frameInput.Catch && !_globalCooldown.IsOnCooldown) {
            Debug.Log("Catch skill has been used.");
            _catch.TriggerCatch();
            _globalCooldown.StartCooldown(SkillType.Catch);

            OnCatch?.Invoke();
        }
    }

    private void HandleScan() {
        if (_frameInput.Scan && !_globalCooldown.IsOnCooldown) {
            Debug.Log("Scan skill has been used.");
            _soundWave.TriggerSoundWave();
            _globalCooldown.StartCooldown(SkillType.Scan);

            OnScan?.Invoke();
        }
    }

    private void HandleCheat() {
        if (_frameInput.Cheat && !_globalCooldown.IsOnCooldown) {
            Debug.Log("Cheat skill has been used.");
            _globalCooldown.StartCooldown(SkillType.Cheat);

            OnCheat?.Invoke(_cheatSkillDuration);
        }
    }

    private void LockControls() {
        _controlsLocked = true;
        _frameInput = new FrameInput();
        _rb.velocity = Vector2.zero;
    }

    private void GameManager_OnGameStarted() {
        LockControls();
        _catch.ResetCatch();
        _soundWave.ResetSoundWave();
    }

    private void GameManager_OnGamePaused() {
        LockControls();
    }

    private void CountdownUI_OnCountdownStep(bool isFinalStep, int stepIndex) {
        if (isFinalStep) {
            _controlsLocked = false;
            Debug.Log("Unlocking Controls.");
            _globalCooldown = new GlobalCooldown(_globalCooldownDuration);
            _globalCooldown.ResetCooldown();
        } else {
            _soundWave.TriggerSoundWave();
        }
    }
}
