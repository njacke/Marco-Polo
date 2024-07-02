using System;
using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour
{    
    public static Action<NPCType> OnCaughtNPC;
    public static Action OnCheatDetected;

    [SerializeField] private Animator _waveAnimator;
    [SerializeField] private SpriteRenderer _caughtVisual;
    [SerializeField] private LayerMask _rcLayerMask;
    [SerializeField] private NPCType _npcType = NPCType.None;
    [SerializeField] private float _moveStateDuration = 2f;
    [SerializeField] private float _idleStateDuration = 1f;
    [SerializeField] private float _idleStateChance = .2f;
    [SerializeField] private float _baseMoveSpeed = 100f;
    [SerializeField] private float _minFleeDist = 1.5f;
    [SerializeField] private float _detectCheatDist = 2.5f;
    
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;

    private CurrentState _currentState = CurrentState.None;
    private float _currentStateTime = 0f;
    private Vector2 _currentMoveDir = Vector2.zero;
    private bool _isCaught = false;

    private readonly int SOUND_WAVE_HASH = Animator.StringToHash("SoundWave");

    private enum CurrentState {
        None,
        Move,
        Idle
    }

    public enum NPCType {
        None,
        Boy1,
        Boy2,
        Girl1,
        Girl2
    }

    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();        
        _currentMoveDir = GetRandomMoveDir();
        _currentState = CurrentState.Move;
    }

    private void OnEnable() {
        PlayerController.OnScan += ScanResponse;
        Blindfold.OnBlindfoldOpened += DetectCheat;
    }

    private void OnDisable() {
        PlayerController.OnScan -= ScanResponse;
        Blindfold.OnBlindfoldOpened -= DetectCheat;
    }

    private void Update() {
        if (!_isCaught && !FleeCheck()) {
            StateUpdate();
        }
    }

    private void FixedUpdate() {
        if (_currentState == CurrentState.None || _currentState == CurrentState.Idle) {
            _rb.velocity = Vector2.zero;
        } else if (_currentState == CurrentState.Move) {
            Move();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.GetComponentInParent<PlayerController>()) {
            HandleCaught();
        }
    }

    private void HandleCaught() {
        Debug.Log("I have been caught.");
        _isCaught = true;
        _caughtVisual.enabled = true;
        _currentState = CurrentState.None;
        OnCaughtNPC?.Invoke(_npcType);

        //_spriteRenderer.enabled = true;
        //Destroy(this.gameObject);
        //Time.timeScale = 0f;
    }

    private void StateUpdate() {
        _currentStateTime += Time.deltaTime;
        if (_currentState == CurrentState.Move && _currentStateTime > _moveStateDuration) {
            StateChangeAuto();
        }
        else if (_currentState == CurrentState.Idle && _currentStateTime > _idleStateDuration) {
            StateChangeAuto();
        }
    }

    private void StateChangeAuto() {
        if (_currentState == CurrentState.Idle || UnityEngine.Random.Range(0f, 1f) > _idleStateChance) {
            _currentState = CurrentState.Move;
            _currentMoveDir = GetRandomMoveDir();
        } else {
            _currentState = CurrentState.Idle;
        }

        _currentStateTime = 0f;
    }

    private void Move() {
        Vector2 movement = _baseMoveSpeed * Time.fixedDeltaTime * _currentMoveDir;
        _rb.velocity = movement;
    }

    private Vector2 GetRandomMoveDir() {
        return new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
    }

    private bool FleeCheck() {
        Vector3 dir = PlayerController.Instance.transform.position - transform.position;
        bool isInFleeRange = dir.magnitude <= _minFleeDist;

        if (isInFleeRange) {
            _currentState = CurrentState.Move;
            _currentMoveDir = -dir.normalized; // opposite of player direction
        }

        return isInFleeRange;
    }

    private void ScanResponse() {
        Debug.Log("Scan response triggered!");
        _waveAnimator.SetTrigger(SOUND_WAVE_HASH);
        //StartCoroutine(ScanResponseRoutine());
    }

    private IEnumerator ScanResponseRoutine() {
        _spriteRenderer.enabled = true;

        yield return new WaitForSeconds(2f);

        _spriteRenderer.enabled = false;
    }

    private void DetectCheat() {
        Vector2 rcDir = PlayerController.Instance.transform.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rcDir, _detectCheatDist, _rcLayerMask);

        if (hit.collider != null && hit.collider.gameObject.GetComponent<PlayerController>()) {
            Debug.Log("Cheating detected.");
            OnCheatDetected?.Invoke();
            Time.timeScale = 0f;
        }
    }
}
