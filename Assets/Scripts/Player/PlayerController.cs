using System;
using System.Collections;
using UnityEditor.Callbacks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static Action OnScan;
    public static Action<float> OnCheat;
    
    [SerializeField] private CircleCollider2D _catchDetector;
    [SerializeField] private SpriteRenderer _catchVisual;
    [SerializeField] private float _globalCooldownDuration = 2f;
    [SerializeField] private float _catchSkillDuration = 1f;
    [SerializeField] private float _cheatSkillDuration = 1f;
    [SerializeField] private float _baseMoveSpeed = 100f;
    
    private Rigidbody2D _rb;
    private PlayerInput _playerInput;
    private SoundWave _soundWave; 
    private GlobalCooldown _globalCooldown;
    private FrameInput _frameInput;

    private bool _controlsLocked = false;

    public enum SkillType {
        None,
        Catch,
        Scan,
        Cheat
    }
    
    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _soundWave = GetComponentInChildren<SoundWave>();
        _globalCooldown = new GlobalCooldown(_globalCooldownDuration);
    }

    private void Start() {
        _catchDetector.enabled = false;
        _catchVisual.enabled = false;     
    }

    private void Update() {
        _globalCooldown.TrackCooldown();

        if (!_controlsLocked) {
            GatherInput();
            HandleCatch();
            HandleScan();
            HandleCheat();
        }
    }

    private void FixedUpdate() {
        if (!_controlsLocked) {
            Move();
        }
    }

    private void OnEnable() {
        GameManager.OnGameStarted += GameManager_OnGameStarted;
        GameManager.OnGamePaused += GameManager_OnGamePaused;
        CountdownUI.OnCountdownStep += CountdownUI_OnCountdownStep;
    }

    private void OnDisable() {
        GameManager.OnGameStarted -= GameManager_OnGameStarted;
        GameManager.OnGamePaused -= GameManager_OnGamePaused;
        CountdownUI.OnCountdownStep -= CountdownUI_OnCountdownStep;
    }

    private void GatherInput() {
        _frameInput = _playerInput.FrameInput;
    }

    private void Move() {
        Vector2 movement = _baseMoveSpeed * Time.fixedDeltaTime * _frameInput.Move;
        //Debug.Log("New movement: " + movement);
        _rb.velocity = movement;
    }

    private void HandleCatch() {
        if (_frameInput.Catch && !_globalCooldown.IsOnCooldown) {
            Debug.Log("Catch skill has been used.");
            StartCoroutine(CatchRoutine());
            _globalCooldown.StartCooldown(SkillType.Catch);
        }
    }

    private IEnumerator CatchRoutine() {
        _catchDetector.enabled = true;
        _catchVisual.enabled = true;
        yield return new WaitForSeconds(_catchSkillDuration);

        _catchDetector.enabled = false;
        _catchVisual.enabled = false;        
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
    }

    private void GameManager_OnGamePaused() {
        LockControls();
    }

    private void CountdownUI_OnCountdownStep(bool isFinalStep, int stepIndex) {
        if (isFinalStep) {
            _controlsLocked = false;
            Debug.Log("Unlocking Controls.");
            _globalCooldown.ResetCooldown();
        } else {
            _soundWave.TriggerSoundWave();
        }
    }
}
