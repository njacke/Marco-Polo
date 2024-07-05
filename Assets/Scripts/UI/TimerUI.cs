using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    public static Action<bool, int> OnTimerStep;

    [SerializeField] private float _timerStartDelay = 0f;
    [SerializeField] private float _timerStepDelay = 1f;
    [SerializeField] private Sprite[] _timerSprites;

    private Image _countdownImage;

    private void Awake() {
        _countdownImage = GetComponent<Image>();
        _countdownImage.sprite = _timerSprites[0];
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

    private void GameManager_OnGameStarted() {
        _countdownImage.sprite = _timerSprites[0];
    }

    private void GameManager_OnGamePaused() {
        StopAllCoroutines();                
    }

    private void CountdownUI_OnCountdownStep(bool isFinalStep, int stepIndex) {
        if (isFinalStep) {
            StartCoroutine(TimerRoutine());
        }
    }

    private IEnumerator TimerRoutine() {
        yield return new WaitForSeconds(_timerStartDelay);
        _countdownImage.sprite = _timerSprites[0];
        OnTimerStep?.Invoke(false, 0);

        for (int i = 1; i < _timerSprites.Length; i++) {
            yield return new WaitForSeconds(_timerStepDelay);
            _countdownImage.sprite = _timerSprites[i];
            if (i == _timerSprites.Length - 1) {
                OnTimerStep?.Invoke(true, i); // final step (GO)
            } else { 
                OnTimerStep?.Invoke(false, i);
            }
        }
    }
}
