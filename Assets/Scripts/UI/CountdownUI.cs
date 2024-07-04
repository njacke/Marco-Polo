using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CountdownUI : MonoBehaviour
{
    public static Action<bool, int> OnCountdownStep;

    [SerializeField] private float _countdownStartDelay = 1f;
    [SerializeField] private float _countdownStepDelay = 1f;
    [SerializeField] private Sprite[] _countdownSprites;

    private Image _countdownImage;

    private void Awake() {
        _countdownImage = GetComponent<Image>();
        _countdownImage.sprite = _countdownSprites[0];
        _countdownImage.enabled = false;
    }

    private void OnEnable() {
        GameManager.OnGameStarted += StartCountdown;
    }

    private void OnDisable() {
        GameManager.OnGameStarted -= StartCountdown;        
    }

    private void StartCountdown() {
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine() {
        yield return new WaitForSeconds(_countdownStartDelay);
        _countdownImage.enabled = true;
        _countdownImage.sprite = _countdownSprites[0];
        OnCountdownStep?.Invoke(false, 0);

        for (int i = 1; i < _countdownSprites.Length; i++) {
            yield return new WaitForSeconds(_countdownStepDelay);
            _countdownImage.sprite = _countdownSprites[i];
            if (i == _countdownSprites.Length - 1) {
                OnCountdownStep?.Invoke(true, i); // final step (GO)
            } else { 
                OnCountdownStep?.Invoke(false, i);
            }
        }

        yield return new WaitForSeconds(_countdownStepDelay);
        
        _countdownImage.enabled = false;
    }
}
