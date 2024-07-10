using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPopUpsUI : MonoBehaviour
{
    public static Action OnIntroSlideComplete;
    public static Action OnMovementSlideDisplayed;
    public static Action OnBlindfoldSlideDiplayed;
    public static Action OnScanSlideDisplayed;
    public static Action OnCheatSlideDisplayed;
    public static Action OnCheatTutorialDone;
    public static Action OnCatchSlideDisplayed;
    public static Action OnGuessSlideDisplayed;
    public static Action OnCatchTutorialDone;
    public static Action OnTutorialCompleted;
    public static Action OnDialogueBoxDisplayed;
    public static Action OnEnableScan;
    public static Action OnEnableCheat;
    public static Action OnEnableCatch;
    public static Action OnMumTutorialDialogue;
    

    [SerializeField] private float _baseLucaDelay = 2f;
    [SerializeField] private GameObject _mainPopUp;
    [SerializeField] private GameObject _lucaPopUp;
    [SerializeField] private Sprite[] _mainSprites;
    [SerializeField] private Sprite[] _lucaSprites;
    [SerializeField] private Vector3 _targetPos2;
    [SerializeField] private Vector3 _startPos2;
    [SerializeField] private SoundWave _mumWave;

    private int _lucaSlideCount = 0;
    private int _mainSlideCount = 0;
    
    private Image _mainPopUpImage;
    private SlideUI _mainPopUpSlideUI;
    private Image _lucaPopUpImage;
    private SlideUI _lucaPopUpSlideUI;

    private bool _cheatDone = false;

    private void Awake() {
        _mainPopUpSlideUI = _mainPopUp.GetComponent<SlideUI>();
        _mainPopUpImage = _mainPopUp.GetComponent<Image>();
        _lucaPopUpSlideUI = _lucaPopUp.GetComponent<SlideUI>();
        _lucaPopUpImage = _lucaPopUp.GetComponent<Image>();
    }

    private void OnEnable() {
        TutorialManager.OnTutorialLoaded += TutorialManager_OnTutorialLoaded;
        TutorialManager.OnLucaMoved += TutorialManager_OnLucaMoved;
        TutorialManager.OnMoveTutorialDone += TutorialManager_OnMoveTutorialDone;
        Blindfold.OnBlinfoldReady += Blinfold_OnBlinfoldReady;
        TutorialManager.OnScanTutorialDone += TutorialManager_OnScanTutorialDone;
        PlayerController.OnCheat += PlayerController_OnCheat;
        TutorialManager.OnFindTutorialDone += TutorialManager_OnFindTutorialDone;
        NPC.OnCaughtNPC += NPC_OnNPCCaught;    
        GuessUI.OnGuessEnd += GuessUI_OnGuessEnd;
    }


    private void OnDisable() {
        TutorialManager.OnTutorialLoaded -= TutorialManager_OnTutorialLoaded;
        TutorialManager.OnLucaMoved -= TutorialManager_OnLucaMoved;
        TutorialManager.OnMoveTutorialDone -= TutorialManager_OnMoveTutorialDone;
        Blindfold.OnBlinfoldReady -= Blinfold_OnBlinfoldReady;
        TutorialManager.OnScanTutorialDone -= TutorialManager_OnScanTutorialDone;
        PlayerController.OnCheat -= PlayerController_OnCheat;
        TutorialManager.OnFindTutorialDone -= TutorialManager_OnFindTutorialDone;
        NPC.OnCaughtNPC -= NPC_OnNPCCaught;        
        GuessUI.OnGuessEnd -= GuessUI_OnGuessEnd;
    }

    private IEnumerator LucaBubbleRoutine(){
        _lucaPopUpImage.sprite = _lucaSprites[_lucaSlideCount];
        _lucaSlideCount++;
        yield return _lucaPopUpSlideUI.SlideInRoutine();
        OnDialogueBoxDisplayed?.Invoke();
        yield return new WaitForSecondsRealtime(_baseLucaDelay);
        yield return _lucaPopUpSlideUI.SlideOutRoutine();
    }

    private void GuessUI_OnGuessEnd(bool isCorrectAnswer) {
        StartCoroutine(OnGuessEnd(isCorrectAnswer));
    }

    private IEnumerator OnGuessEnd(bool isCorrectAnswer) {
        if (isCorrectAnswer) {
            // tutorial done we can play pog
            yield return StartCoroutine(LucaBubbleRoutine());
            // 2nd pog
            yield return StartCoroutine(LucaBubbleRoutine());

            OnTutorialCompleted?.Invoke();
        }
    }


    private void NPC_OnNPCCaught(NPC npc) {
        StartCoroutine(OnNPCCaught(npc));
    }

    private IEnumerator OnNPCCaught(NPC npc) {
        yield return _mainPopUpSlideUI.SlideOutRoutine();   

        // guess me
        yield return StartCoroutine(LucaBubbleRoutine());
        OnCatchTutorialDone?.Invoke();      
    }

    private void TutorialManager_OnFindTutorialDone() {
        StartCoroutine(OnFindTutorialDone());
    }

    private IEnumerator OnFindTutorialDone() {
        yield return _mainPopUpSlideUI.SlideOutRoutine();   

        // catch me
        yield return StartCoroutine(LucaBubbleRoutine());

        OnEnableCatch?.Invoke();

       _mainPopUpImage.sprite = _mainSprites[_mainSlideCount];
        yield return _mainPopUpSlideUI.SlideInRoutine(); // use F
        OnCatchSlideDisplayed?.Invoke();
    }

    private void PlayerController_OnCheat(float duration) {
        if (_cheatDone) {
            return;
        }

        StartCoroutine(OnCheat(duration));
    }

    private IEnumerator OnCheat(float duration) {
        _cheatDone = true;
        yield return new WaitForSeconds(duration); // blind closes
        yield return _mainPopUpSlideUI.SlideOutRoutine();

        // just make sure noone is around
        yield return StartCoroutine(LucaBubbleRoutine());

        // can use any means to find me; i won't tell
        yield return StartCoroutine(LucaBubbleRoutine());


       _mainPopUpImage.sprite = _mainSprites[_mainSlideCount];
        _mainSlideCount++;
        yield return _mainPopUpSlideUI.SlideInRoutine(); // find Luca
        OnCheatTutorialDone?.Invoke();
    }

    private void TutorialManager_OnScanTutorialDone() {
        StartCoroutine(OnScanTutorialDone());
    }

    private IEnumerator OnScanTutorialDone() {
         //hit mid collider -> start cheat tutorial
        yield return _mainPopUpSlideUI.SlideOutRoutine();

        yield return StartCoroutine(LucaBubbleRoutine());

        yield return StartCoroutine(LucaBubbleRoutine());

        OnEnableCheat?.Invoke();

        _mainPopUpImage.sprite = _mainSprites[_mainSlideCount];
        _mainSlideCount++;
        yield return _mainPopUpSlideUI.SlideInRoutine(); // use Q
        OnCheatSlideDisplayed?.Invoke();
    }

    private void Blinfold_OnBlinfoldReady() {
        StartCoroutine(OnBlinfoldReady());
    }

    private IEnumerator OnBlinfoldReady() {
        yield return _mainPopUpSlideUI.SlideOutRoutine();
        _mumWave.TriggerSoundWave();
        OnMumTutorialDialogue?.Invoke();
        yield return new WaitForSecondsRealtime(_baseLucaDelay); // luca moved to new pos

        _lucaPopUp.transform.position = _startPos2;
        _lucaPopUpSlideUI.SetStartAndTargetPos(_startPos2, _targetPos2);       

        yield return StartCoroutine(LucaBubbleRoutine());

        yield return StartCoroutine(LucaBubbleRoutine());

        OnEnableScan?.Invoke();
        _mainPopUpImage.sprite = _mainSprites[_mainSlideCount];
        _mainSlideCount++;
        yield return _mainPopUpSlideUI.SlideInRoutine(); // use E
        OnScanSlideDisplayed?.Invoke();
    }

    private void TutorialManager_OnMoveTutorialDone() {
        StartCoroutine(OnMoveTutorialDone());
    }

    private IEnumerator OnMoveTutorialDone() {
        yield return _mainPopUpSlideUI.SlideOutRoutine();

        // let's play
        yield return StartCoroutine(LucaBubbleRoutine());

        // fav game
        yield return StartCoroutine(LucaBubbleRoutine());

        // close eyes
        yield return StartCoroutine(LucaBubbleRoutine());


        _mainPopUpImage.sprite = _mainSprites[_mainSlideCount];
        _mainSlideCount++;
        yield return _mainPopUpSlideUI.SlideInRoutine(); // press Q for ready
        OnBlindfoldSlideDiplayed?.Invoke();
    }

    private void TutorialManager_OnLucaMoved() {
        StartCoroutine(OnLucaMoved());
    }
    
    private IEnumerator OnLucaMoved() {
        // come closer
        yield return StartCoroutine(LucaBubbleRoutine());

        _mainPopUpImage.sprite = _mainSprites[_mainSlideCount];
        _mainSlideCount++;
        yield return _mainPopUpSlideUI.SlideInRoutine();
        OnMovementSlideDisplayed?.Invoke();
    }

    private void TutorialManager_OnTutorialLoaded() {
        StartCoroutine(OnTutorialLoaded());
    }

    private IEnumerator OnTutorialLoaded() {
        _mainPopUpImage.sprite = _mainSprites[0];
        _mainSlideCount++;
        //yield return _mainPopUpSlideUI.SlideInRoutine();
        yield return new WaitForSecondsRealtime(_baseLucaDelay);
        //yield return _mainPopUpSlideUI.SlideOutRoutine();
        
        OnIntroSlideComplete?.Invoke();
    }
}

