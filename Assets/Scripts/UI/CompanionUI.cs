using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CompanionUI : MonoBehaviour
{
    public static Action OnContinue;
    public static Action OnCompanionDisplayed;

    [SerializeField] float _resetDelay = 1f;
    [SerializeField] Image _titleImage;
    [SerializeField] Image _companionImage;
    [SerializeField] Image _npcName;
    [SerializeField] Button _continueButton;
    [SerializeField] Sprite _itaImage;
    [SerializeField] Sprite _gerImage;
    [SerializeField] Sprite __spaImage;
    [SerializeField] Sprite _usaImage;
    [SerializeField] Sprite _itaName;
    [SerializeField] Sprite _gerName;
    [SerializeField] Sprite __spaName;
    [SerializeField] Sprite _usaName;

    private CursorManager _cursorManager;

    private void Awake() {
        Init();        
    }

    private void OnEnable() {
        WinScreenUI.OnContinue += WinScreenUI_OnContinue;
    }

    private void WinScreenUI_OnContinue() {

        bool imageFound = false;

        if (TutorialManager.Instance != null) {
            _companionImage.sprite = _gerImage;
            _npcName.sprite = _gerName;
            imageFound = true;
        } else if (GameManager.Instance != null) {
            switch (GameManager.Instance.GetActiveLevel) {
                case GameManager.Level.Pool:
                    _companionImage.sprite = __spaImage;
                    _npcName.sprite = __spaName;
                    imageFound = true;
                    break;
                case GameManager.Level.Garden:
                    _companionImage.sprite = _usaImage;
                    _npcName.sprite = _usaName;
                    imageFound = true;
                    break;
                default:
                    break;
            }
        }        

        if (!imageFound) {
            OnContinue?.Invoke();
            return;
        }

        StartCoroutine(LoadCompanionUIRoutine());
    }

    private void OnDisable() {
        WinScreenUI.OnContinue -= WinScreenUI_OnContinue;
    }


    private void Init() {
        _cursorManager = GetComponentInChildren<CursorManager>();
    }

    private IEnumerator LoadCompanionUIRoutine() {
        var companionSlideUI = _companionImage.GetComponent<SlideUI>();    
        yield return companionSlideUI.SlideInRoutine();

        OnCompanionDisplayed?.Invoke();

        var titleSlideUI = _titleImage.GetComponent<SlideUI>();
        yield return titleSlideUI.SlideInRoutine();

        var continueSlideUI = _continueButton.GetComponent<SlideUI>();
        yield return continueSlideUI.SlideInRoutine();  

        _cursorManager.gameObject.SetActive(true);
        _cursorManager.DisplayAndEnableCursor();    
    }

    private IEnumerator ResetCompanionUIRoutine() {
        yield return new WaitForSecondsRealtime(_resetDelay);
        _cursorManager.DisableCursor();
        _cursorManager.gameObject.SetActive(false);

        var trophySlideUI = _companionImage.GetComponent<SlideUI>();
        StartCoroutine(trophySlideUI.SlideOutRoutine());

        var titleSlideUI = _titleImage.GetComponent<SlideUI>();
        StartCoroutine(titleSlideUI.SlideOutRoutine());

        var continueSlideUI = _continueButton.GetComponent<SlideUI>();
        yield return continueSlideUI.SlideOutRoutine();      
    }

    public void OnContinueSelected() {
        StartCoroutine(OnContinueSelectedRoutine());
    }

    public IEnumerator OnContinueSelectedRoutine() {
        yield return StartCoroutine(ResetCompanionUIRoutine());
        OnContinue?.Invoke();
    }
}
