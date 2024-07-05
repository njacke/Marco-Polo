using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WinScreenUI : MonoBehaviour
{
    public static Action OnContinue;

    [SerializeField] Image _titleImage;
    [SerializeField] Image _trophyImage;
    [SerializeField] Button _continueButton;

    private void Awake() {
        Init();        
    }

    private void OnEnable() {
        GameManager.OnLevelCompleted += GameManager_OnLevelCompleted;
    }

    private void OnDisable() {
        GameManager.OnLevelCompleted -= GameManager_OnLevelCompleted;
    }

    private void Init() {

    }

    private void GameManager_OnLevelCompleted() {
        StartCoroutine(LoadWinScreenUIRoutine());
    }

    private IEnumerator LoadWinScreenUIRoutine() {
        var trophySlideUI = _trophyImage.GetComponent<SlideUI>();
        yield return trophySlideUI.SlideInRoutine();

        var titleSlideUI = _titleImage.GetComponent<SlideUI>();
        yield return titleSlideUI.SlideInRoutine();

        var continueSlideUI = _continueButton.GetComponent<SlideUI>();
        yield return continueSlideUI.SlideInRoutine();      
    }

    private IEnumerator ResetWinScreenUIRoutine() {
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
