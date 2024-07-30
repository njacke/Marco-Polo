using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicManager : MonoBehaviour
{
    public static Action OnCinematicLoaded;

    [SerializeField] float _cameraTargetSize = 12f;
    [SerializeField] float _cameraPanOutDur = 5f;

    private Camera _mainCamera;

    private void Start() {
        OnCinematicLoaded?.Invoke();
        _mainCamera = Camera.main;
    }

    public void LoadNextLevel() {
        if (GameManager.Instance != null) {
            GameManager.Instance.LoadActiveLevel();
        }
    }

    public void ItaSpeech() {
        AudioManager.Instance.ItaSpeech();
    }

    public void GerSpeech() {
        AudioManager.Instance.GerSpeech();
    }

    public void SpaSpeech() {
        AudioManager.Instance.SpaSpeech();
    }

    public void UsaSpeech() {
        AudioManager.Instance.UsaSpeech();
    }

    public void PanOutCamera() {
        StartCoroutine(PanOutCameraRoutine());
    }

    private IEnumerator PanOutCameraRoutine() {
        float startSize = _mainCamera.orthographicSize;
        float elapsedTime = 0f;

        while (elapsedTime < _cameraPanOutDur)
        {
            _mainCamera.orthographicSize = Mathf.Lerp(startSize, _cameraTargetSize, elapsedTime / _cameraPanOutDur);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _mainCamera.orthographicSize = _cameraTargetSize;
    }
}
