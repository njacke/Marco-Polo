using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public static Action OnRetry;

    [SerializeField] Image _titleImage;
    [SerializeField] private Sprite[] _titleSprites;
    [SerializeField] Image _skullImage;
    [SerializeField] Button _retryButton;

    private Dictionary<GameManager.GameOverType, Sprite> _titleSpritesDict;

    private void Awake() {
        Init();        
    }

    private void OnEnable() {
        GameManager.OnGameOver += GameManager_OnGameOver;
    }

    private void OnDisable() {
        GameManager.OnGameOver -= GameManager_OnGameOver;
    }

    private void Init() {
        _titleSpritesDict = new Dictionary<GameManager.GameOverType, Sprite>() {
            { GameManager.GameOverType.Timer, _titleSprites[0] },
            { GameManager.GameOverType.Cheat, _titleSprites[1] },
            { GameManager.GameOverType.Bite, _titleSprites[2] }
        };
    }

    private void GameManager_OnGameOver(GameManager.GameOverType type) {
        StartCoroutine(LoadGameOverUIRoutine(type));
    }

    private IEnumerator LoadGameOverUIRoutine(GameManager.GameOverType type) {
        var skullSlideUI = _skullImage.GetComponent<SlideUI>();
        yield return skullSlideUI.SlideInRoutine();

        _titleImage.sprite = _titleSpritesDict[type];
        var titleSlideUI = _titleImage.GetComponent<SlideUI>();
        yield return titleSlideUI.SlideInRoutine();

        var retrySlideUI = _retryButton.GetComponent<SlideUI>();
        yield return retrySlideUI.SlideInRoutine();      
    }

    private IEnumerator ResetGameOverUIRoutine() {
        var skullSlideUI = _skullImage.GetComponent<SlideUI>();
        StartCoroutine(skullSlideUI.SlideOutRoutine());

        var titleSlideUI = _titleImage.GetComponent<SlideUI>();
        StartCoroutine(titleSlideUI.SlideOutRoutine());

        var retrySlideUI = _retryButton.GetComponent<SlideUI>();
        yield return retrySlideUI.SlideOutRoutine();      
    }

    public void OnRetrySelected() {
        StartCoroutine(OnRetrySelectedRoutine());
    }

    public IEnumerator OnRetrySelectedRoutine() {
        yield return StartCoroutine(ResetGameOverUIRoutine());
        OnRetry?.Invoke();
    }
}
