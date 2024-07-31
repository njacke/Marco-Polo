using System;
using System.Collections;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public static Action OnMainMenuLoaded;

    [SerializeField] private float _loadTutorialDelay = .5f; 

    private void Start() {
        OnMainMenuLoaded?.Invoke();
    }

    public void OnPlaySelected() {
        StartCoroutine(LoadTutorialRoutine());
    }

    private IEnumerator LoadTutorialRoutine() {
        yield return new WaitForSecondsRealtime(_loadTutorialDelay);

        var transitionUI = FindObjectOfType<TransitionUI>();
        if (transitionUI != null) {
            yield return transitionUI.FadeInRoutine();
        }
        
        SceneLoader.LoadSceneAsync(SceneLoader.SCENE_TUTORIAL_STRING);
    }
}
