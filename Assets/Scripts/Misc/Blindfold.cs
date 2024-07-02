using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blindfold : MonoBehaviour
{
    public static Action OnBlindfoldOpened;

    [SerializeField] private Transform _topPart;
    [SerializeField] private Transform _botPart;
    [SerializeField] private float _minY = 2f;
    [SerializeField] private float _maxY = 8.5f;

    private Coroutine _blindfoldRoutine;

    private void Start() {
        CloseBlindfold(2f);
    }

    private void OnEnable() {
        PlayerController.OnCheat += OpenAndCloseBlindfold;
    }

    private void OnDisable() {
        PlayerController.OnCheat -= OpenAndCloseBlindfold;        
    }

    private void CloseBlindfold(float duration) {
        StartCoroutine(CloseBlindfoldRoutine(duration));
    }

    private void OpenAndCloseBlindfold(float duration) {
        _blindfoldRoutine ??= StartCoroutine(OpenAndCloseBlindfoldRoutine(duration));
    }

    private IEnumerator OpenAndCloseBlindfoldRoutine(float duration) {
        yield return StartCoroutine(OpenBlindfoldRoutine(duration / 2));
        yield return StartCoroutine(CloseBlindfoldRoutine(duration / 2));
        _blindfoldRoutine = null;
    }

    private IEnumerator CloseBlindfoldRoutine(float duration) {
        float timePassed = 0f;
        float startPosYTop = _topPart.position.y;
        float startPosYBot = _botPart.position.y;

        while (_topPart.position.y > _minY && _botPart.position.y < -_minY) {
            timePassed += Time.deltaTime;
            var linearT = timePassed / duration;
            float newPosYTop = Mathf.Lerp(startPosYTop, _minY, linearT);
            float newPosYBot = Mathf.Lerp(startPosYBot, -_minY, linearT);

            _topPart.transform.position = new Vector3(_topPart.transform.position.x, newPosYTop, _topPart.transform.position.z);
            _botPart.transform.position = new Vector3(_botPart.transform.position.x, newPosYBot, _botPart.transform.position.z);

            yield return null;
        }
    }

    private IEnumerator OpenBlindfoldRoutine(float duration) {
        float timePassed = 0f;
        float startPosYTop = _topPart.position.y;
        float startPosYBot = _botPart.position.y;

        while (_topPart.position.y < _maxY && _botPart.position.y > -_maxY) {
            timePassed += Time.deltaTime;
            var linearT = timePassed / duration;
            float newPosYTop = Mathf.Lerp(startPosYTop, _maxY, linearT);
            float newPosYBot = Mathf.Lerp(startPosYBot, -_maxY, linearT);

            _topPart.transform.position = new Vector3(_topPart.transform.position.x, newPosYTop, _topPart.transform.position.z);
            _botPart.transform.position = new Vector3(_botPart.transform.position.x, newPosYBot, _botPart.transform.position.z);

            yield return null;
        }

        OnBlindfoldOpened?.Invoke();
    }
}
