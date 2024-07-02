using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static Action OnScan;
    public static Action<float> OnCheat;
    public static PlayerController Instance;
    
    [SerializeField] private CircleCollider2D _catchDetector;
    [SerializeField] private SpriteRenderer _catchVisual;
    [SerializeField] private Animator _waveAnimator;
    [SerializeField] private float _globalCooldownDuration = 2f;
    [SerializeField] private float _catchSkillDuration = 1f;
    [SerializeField] private float _cheatSkillDuration = 1f;
    [SerializeField] private float _baseMoveSpeed = 100f;
    
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private PlayerInput  _playerInput;
    private FrameInput _frameInput;
    private GlobalCooldown _globalCooldown;

    private bool _controlsLocked = false;

    private readonly int SOUND_WAVE_HASH = Animator.StringToHash("SoundWave");

    public enum SkillType {
        None,
        Catch,
        Scan,
        Cheat
    }
    
    private void Awake() {
        if (Instance == null) { Instance = this; }

        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerInput = GetComponent<PlayerInput>();
        _globalCooldown = new GlobalCooldown(_globalCooldownDuration);
    }

    private void Start() {
        //_spriteRenderer.enabled = false;
        _catchDetector.enabled = false;
        _catchVisual.enabled = false;        
    }

    private void Update() {
        _globalCooldown.TrackCooldown();

        GatherInput();
        Move();
        
        HandleCatch();
        HandleScan();
        HandleCheat();
    }

    private void FixedUpdate() {
        Move();
    }

    private void OnEnable() {
        GameManager.OnGamePaused += HandleGamePaused;
    }

    private void OnDisable() {
        GameManager.OnGamePaused -= HandleGamePaused;
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
            _waveAnimator.SetTrigger(SOUND_WAVE_HASH);
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

    private void HandleGamePaused() {
        _controlsLocked = true;
        _globalCooldown.ResetCooldown();
    }
}
