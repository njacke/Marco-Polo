using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WinScreenUI : MonoBehaviour
{
    public static Action OnContinue;
    public static Action OnTrophyDisplayed;

    [SerializeField] float _resetDelay = 1f;
    [SerializeField] Image _titleImage;
    [SerializeField] Image _trophyImage;
    [SerializeField] Button _continueButton;
    private CursorManager _cursorManager;

    private void Awake() {
        Init();        
    }

    private void OnEnable() {
        GameManager.OnLevelCompleted += GameManager_OnLevelCompleted;

        TutorialPopUpsUI.OnTutorialCompleted += TutorialPopUpsUI_OnTutorialCompleted;
    }

    private void OnDisable() {
        GameManager.OnLevelCompleted -= GameManager_OnLevelCompleted;

        TutorialPopUpsUI.OnTutorialCompleted -= TutorialPopUpsUI_OnTutorialCompleted;
    }


    private void Init() {
        _cursorManager = GetComponentInChildren<CursorManager>();
    }

    private void TutorialPopUpsUI_OnTutorialCompleted() {
        StartCoroutine(LoadWinScreenUIRoutine());
    }

    private void GameManager_OnLevelCompleted() {
        StartCoroutine(LoadWinScreenUIRoutine());
    }

    private IEnumerator LoadWinScreenUIRoutine() {
        var trophySlideUI = _trophyImage.GetComponent<SlideUI>();
        yield return trophySlideUI.SlideInRoutine();

        OnTrophyDisplayed?.Invoke();

        var titleSlideUI = _titleImage.GetComponent<SlideUI>();
        yield return titleSlideUI.SlideInRoutine();

        var continueSlideUI = _continueButton.GetComponent<SlideUI>();
        yield return continueSlideUI.SlideInRoutine();  

        _cursorManager.gameObject.SetActive(true);
        _cursorManager.DisplayAndEnableCursor();    
    }

    private IEnumerator ResetWinScreenUIRoutine() {
        yield return new WaitForSecondsRealtime(_resetDelay);
        _cursorManager.DisableCursor();
        _cursorManager.gameObject.SetActive(false);

        var trophySlideUI = _trophyImage.GetComponent<SlideUI>();
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
        yield return StartCoroutine(ResetWinScreenUIRoutine());
        OnContinue?.Invoke();
    }
}
