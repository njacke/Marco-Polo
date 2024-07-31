using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicManager : MonoBehaviour
{
    public static Action OnCinematicLoaded;
    public static Action<float> OnEpilogueEnd;

    [SerializeField] float _cameraTargetSize = 12f;
    [SerializeField] Vector3 _cameraTargetPos;
    [SerializeField] float _cameraPanOutDur = 5f;
    [SerializeField] float _endGameDelay = 3f;

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
        //AudioManager.Instance.ItaSpeech();
    }

    public void GerSpeech() {
        //AudioManager.Instance.GerSpeech();
    }

    public void SpaSpeech() {
        //AudioManager.Instance.SpaSpeech();
    }

    public void UsaSpeech() {
        //AudioManager.Instance.UsaSpeech();
    }

    public void EpilogueEnd() {
        StartCoroutine(EpilogueEndRoutine());
        OnEpilogueEnd?.Invoke(_cameraPanOutDur + _endGameDelay);
    }

    private IEnumerator EpilogueEndRoutine() {
        yield return PanOutCameraRoutine();
        yield return new WaitForSecondsRealtime(_endGameDelay);

        var transitionUI = FindObjectOfType<TransitionUI>();
        if (transitionUI != null) {
            yield return transitionUI.FadeInRoutine();
        }
        
        SceneLoader.LoadSceneAsync(SceneLoader.SCENE_END_MENU_STRING);
    }

    private IEnumerator PanOutCameraRoutine() {
        float startSize = _mainCamera.orthographicSize;
        Vector3 startPos = _mainCamera.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < _cameraPanOutDur) {
            _mainCamera.orthographicSize = Mathf.Lerp(startSize, _cameraTargetSize, elapsedTime / _cameraPanOutDur);
            _mainCamera.transform.position = Vector3.Lerp(startPos, _cameraTargetPos, elapsedTime / _cameraPanOutDur);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _mainCamera.orthographicSize = _cameraTargetSize;
        _mainCamera.transform.position = _cameraTargetPos;
    }
}
