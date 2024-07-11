using System;
using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static Action OnGameStarted;
    public static Action OnGamePaused;
    public static Action<NPC.NPCType> OnGuess;
    public static Action OnContestantEliminated;


    public static Action OnTutorialLoaded;
    public static Action OnMoveTutorialDone;
    public static Action OnLucaMoved;
    public static Action OnScanTutorialDone;
    public static Action OnFindTutorialDone;

    public static TutorialManager Instance;

    [SerializeField] private NPC _lucaNPC;
    [SerializeField] private GameObject _mum;
    [SerializeField] private GameObject _dad;

    // TUTORIAL
    [SerializeField] private float _introMoveTime = 2f;
    [SerializeField] private Vector3 _introTargetPos;
    [SerializeField] private Vector3 _finalTargetPos;
    [SerializeField] private Collider2D _moveCollider;
    [SerializeField] private Collider2D _scanCollider;
    [SerializeField] private Collider2D _catchCollider;
    private bool _moveTutorialComplete = false;
    private bool _scanTutorialComplete = false;
    private bool _findTutorialComplete = false;

    public NPC GetTutorialNPC { get => _lucaNPC; }

    private void Awake() {
        if (Instance == null) { Instance = this; }
        _scanCollider.enabled = false;
        _catchCollider.enabled = false;
    }


    private void Start() {
        OnTutorialLoaded?.Invoke();
    }

    private void OnEnable() {
        TutorialPopUpsUI.OnIntroSlideComplete += TutorialPopUpsUI_OnIntroSlideComplete;
        TutorialPopUpsUI.OnEnableScan += TutorialPopUpsUI_OnEnableScan;
        Blindfold.OnBlinfoldReady += Blindfold_OnBlindfoldReady;
        NPC.OnCaughtNPC += NPC_OnNPCCaught; 
        GuessUI.OnGuessEnd += GuessUI_OnGuessEnd;
        CompanionUI.OnContinue += CompanionUI_OnContinue;
    }

    private void OnDisable() {
        TutorialPopUpsUI.OnIntroSlideComplete -= TutorialPopUpsUI_OnIntroSlideComplete;
        TutorialPopUpsUI.OnEnableScan -= TutorialPopUpsUI_OnEnableScan;
        Blindfold.OnBlinfoldReady -= Blindfold_OnBlindfoldReady;
        NPC.OnCaughtNPC -= NPC_OnNPCCaught;        
        GuessUI.OnGuessEnd -= GuessUI_OnGuessEnd;
        CompanionUI.OnContinue -= CompanionUI_OnContinue;
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.GetComponent<PlayerController>()) {
            if (!_moveTutorialComplete) {
                Debug.Log("Move tutorial complete");
                _moveTutorialComplete = true;
                _moveCollider.enabled = false;
                _scanCollider.enabled = true;
                OnMoveTutorialDone?.Invoke();
            } else if (!_scanTutorialComplete){
                _scanTutorialComplete = true;
                _scanCollider.enabled = false;
                _catchCollider.enabled = true;
                OnScanTutorialDone?.Invoke();                
            } else if (!_findTutorialComplete) {
                _catchCollider.enabled = false;
                OnFindTutorialDone?.Invoke();
            }
        }
    }

    private void GuessUI_OnGuessEnd(bool isCorrectAnswer) {
        if (isCorrectAnswer) {
            Time.timeScale = 1f;
        }
    }
    private void TutorialPopUpsUI_OnEnableScan() {
        _mum.SetActive(false);
        _dad.SetActive(false);
    }
    
    private void Blindfold_OnBlindfoldReady() {
        _lucaNPC.transform.position = _finalTargetPos;
    }

    private void TutorialPopUpsUI_OnIntroSlideComplete() {
        StartCoroutine(OnIntroSlideComplete());
    }

    private IEnumerator OnIntroSlideComplete() {
        yield return MoveToTarget(_lucaNPC.gameObject, _introTargetPos, _introMoveTime);
        OnLucaMoved?.Invoke();
    }
    

    private IEnumerator MoveToTarget(GameObject gameObject, Vector3 targetPos, float moveTime) {
        float elapsedTime = 0f;
        var startPos = gameObject.transform.position;

        while (elapsedTime < moveTime) {
            float t = elapsedTime / moveTime;
            gameObject.transform.position = Vector3.Lerp(startPos, targetPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        gameObject.transform.position = targetPos;
    }

    private void NPC_OnNPCCaught(NPC sender) {
        Time.timeScale = 0f;
    }

    private void CompanionUI_OnContinue() {
        SceneLoader.LoadScene("Game");
    }
}
