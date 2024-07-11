using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameUI : MonoBehaviour
{
    public static Action OnEndGameMenuLoaded;

    [SerializeField] private float _loadTutorialDelay = .5f; 

    private void Start() {
        OnEndGameMenuLoaded?.Invoke();
    }

    public void OnMenuSelected() {
        StartCoroutine(LoadMainMenuRoutine());
    }

    private IEnumerator LoadMainMenuRoutine() {
        yield return new WaitForSecondsRealtime(_loadTutorialDelay);
        SceneLoader.LoadSceneAsync(SceneLoader.SCENE_MAIN_MENU_STRING);
    }
}
