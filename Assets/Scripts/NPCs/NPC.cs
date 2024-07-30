using System;
using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour
{    
    public static Action<NPC> OnCaughtNPC;
    public static Action<NPC, bool> OnScanResponse;
    public static Action<NPC, bool> OnCheatDetected;
    public static Action<NPC> OnRandomVoice;

    public NPCType GetNPCType { get => _npcType; }
    public bool IsContestant { get => _isContestant; }

    [SerializeField] private SpriteRenderer _caughtVisual;
    [SerializeField] private Transform _cheatVisual;
    [SerializeField] private float _cheatVisualDuration = 1f;
    [SerializeField] private float _cheatVisualTargetY = 1f;
    [SerializeField] private LayerMask _rcLayerMask;
    [SerializeField] private NPCType _npcType = NPCType.None;
    [SerializeField] private bool _isContestant = true;
    [SerializeField] private float _moveStateDuration = 2f;
    [SerializeField] private float _idleStateDuration = 1f;
    [SerializeField] private float _idleStateChance = .2f;
    [SerializeField] private float _baseMoveSpeed = 100f;
    [SerializeField] private float _minFleeDist = 1.5f;
    [SerializeField] private float _fleeDirAngle = 45f;
    [SerializeField] private float _detectCheatDist = 2.5f;
    [SerializeField] private float _minResponseDelay = .5f;
    [SerializeField] private float _maxResponseDelay = 1f;
    [SerializeField] private float _rndVoiceCD = 2f;
    [SerializeField] private float _rndVoiceCDrange = .5f;
    [SerializeField] private float _rndVoiceChance = .2f;
    
    private Rigidbody2D _rb;
    private SoundWave _soundWave;

    private CurrentState _currentState = CurrentState.None;
    private float _currentStateTime = 0f;
    private Vector2 _currentMoveDir = Vector2.zero;
    private bool _movementLocked = true;
    private bool _isCaught = false;
    private float _rndVoiceCDRemaining = 0f;

    private bool _isTutorial = false;

    private enum CurrentState {
        None,
        Move,
        Idle
    }

    public enum NPCType {
        None,
        Italian,
        German,
        Spaniard,
        American,
        Dog
    }

    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _soundWave = GetComponentInChildren<SoundWave>();

        _currentMoveDir = GetRandomMoveDir();
        _currentState = CurrentState.Move;
        _rndVoiceCDRemaining = _rndVoiceCD;
    }

    private void Start() {
        if (TutorialManager.Instance != null) {
            _isTutorial = true;
        }

        _rndVoiceCDRemaining = UnityEngine.Random.Range(_rndVoiceCD - _rndVoiceCDrange, _rndVoiceCD + _rndVoiceCDrange);
    }

    private void OnEnable() {
        GameManager.OnGameStarted += GameManager_OnGameStarted;
        PlayerController.OnScan += PlayerController_OnScan;
        Blindfold.OnBlindfoldOpened += Blindfold_OnBlinfoldOpened;
        TutorialPopUpsUI.OnTutorialCompleted += TutorialPopUpsUI_OnTutorialCompleted;
        TutorialPopUpsUI.OnDialogueBoxDisplayed += TutorialPopUpsUI_OnDialogueBoxDisplayed;
    }


    private void OnDisable() {
        GameManager.OnGameStarted -= GameManager_OnGameStarted;
        PlayerController.OnScan -= PlayerController_OnScan;
        Blindfold.OnBlindfoldOpened -= Blindfold_OnBlinfoldOpened;
        TutorialPopUpsUI.OnTutorialCompleted -= TutorialPopUpsUI_OnTutorialCompleted;
        TutorialPopUpsUI.OnDialogueBoxDisplayed += TutorialPopUpsUI_OnDialogueBoxDisplayed;
    }
    

    private void Update() {
        if (!_isTutorial && !_isCaught && !FleeCheck()) {
           StateUpdate();    
        } 

        if (!_isTutorial) {
            TryUseRandomVoice();
        }
    }


    private void FixedUpdate() {
        if (!_movementLocked) {
            if (_currentState == CurrentState.None || _currentState == CurrentState.Idle) {
                _rb.velocity = Vector2.zero;
            } else if (_currentState == CurrentState.Move) {
                Move();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (_npcType != NPCType.None && other.gameObject.GetComponentInParent<PlayerController>()) {
            HandleCaught();
        }
    }

    private void HandleCaught() {
        Debug.Log("I have been caught.");
        _isCaught = true;
        _caughtVisual.enabled = true;
        _currentState = CurrentState.None;
        OnCaughtNPC?.Invoke(this);
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
        Vector3 dir = GameManager.Instance.GetActivePlayer.transform.position - transform.position;
        bool isInFleeRange = dir.magnitude <= _minFleeDist;

        if (isInFleeRange) {
            _currentState = CurrentState.Move;
            var rndAngle = UnityEngine.Random.Range(-_fleeDirAngle, _fleeDirAngle);
            Quaternion rotation = Quaternion.AngleAxis(rndAngle, Vector3.forward);
            var newDir = rotation * -dir.normalized;
            _currentMoveDir = newDir;
        }

        return isInFleeRange;
    }

    private void TryUseRandomVoice() {
        _rndVoiceCDRemaining -= Time.deltaTime;
        if (_rndVoiceCDRemaining <= 0f) {
            if (UnityEngine.Random.Range(0f, 1f) < _rndVoiceChance &&_soundWave.TriggerSoundWave()) {
                Debug.Log("Using random voice.");
                OnRandomVoice?.Invoke(this);          
            }
            _rndVoiceCDRemaining = UnityEngine.Random.Range(_rndVoiceCD - _rndVoiceCDrange, _rndVoiceCD + _rndVoiceCDrange);
        }
    }

    private void TutorialPopUpsUI_OnDialogueBoxDisplayed() {
        _soundWave.TriggerSoundWave();
    }

    private void TutorialPopUpsUI_OnTutorialCompleted() {
        _isCaught = false;
        _caughtVisual.enabled = false;
        _soundWave.ResetSoundWave();
    }
    private void GameManager_OnGameStarted() {
        StopAllCoroutines(); // scan response has possible delay
        _isCaught = false;
        _caughtVisual.enabled = false;
        _movementLocked = false;
        _soundWave.ResetSoundWave();
    }

    private void PlayerController_OnScan() {
        if (!_isContestant) return; // non-contestants/tutorial don't respond

        StartCoroutine(HandleOnScanRoutine());        
    }

    private IEnumerator HandleOnScanRoutine() {
        Debug.Log("On Scan routine triggered");
        var delay = UnityEngine.Random.Range(_minResponseDelay, _maxResponseDelay);
        yield return new WaitForSeconds(delay);
        if (_soundWave.TriggerSoundWave()) {
            OnScanResponse?.Invoke(this, true);
        } else {
            OnScanResponse?.Invoke(this, false);
        }
        _rndVoiceCDRemaining = _rndVoiceCD;
    }

    private void Blindfold_OnBlinfoldOpened() {
        if(!_isContestant || _isTutorial) return; // non-contestants/tutorial don't detect cheating

        Vector2 rcDir = GameManager.Instance.GetActivePlayer.transform.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rcDir, _detectCheatDist, _rcLayerMask);

        // if player in cheat detection range + visible
        if (hit.collider != null && hit.collider.gameObject.GetComponent<PlayerController>()) {
            Debug.Log("Cheating detected.");
            
            if (_soundWave.TriggerSoundWave()) {
                OnCheatDetected?.Invoke(this, true);
            } else {
                OnCheatDetected?.Invoke(this, false);
            }
            _rndVoiceCDRemaining = _rndVoiceCD;
        }
    }

    public IEnumerator DisplayCheatVisual() {
        _cheatVisual.gameObject.SetActive(true);
        Vector3 startPos = _cheatVisual.localPosition;
        Vector3 targetPos = startPos + new Vector3(0f, _cheatVisualTargetY, 0f);

        float timePassed = 0f;

        while (_cheatVisual.localPosition != targetPos) {
            timePassed += Time.deltaTime;
            var linearT = timePassed / _cheatVisualDuration;
            Vector3 newLocalPos = Vector3.Lerp(startPos, targetPos, linearT);

            _cheatVisual.localPosition = newLocalPos;

            yield return null;
        }
    }
}
