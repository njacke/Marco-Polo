using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private float _loadTutorialDelay = .5f; 

    public void OnPlaySelected() {
        StartCoroutine(LoadTutorialRoutine());
    }

    private IEnumerator LoadTutorialRoutine() {
        yield return new WaitForSecondsRealtime(_loadTutorialDelay);
        SceneLoader.LoadSceneAsync(SceneLoader.SCENE_TUTORIAL_STRING);
    }
}
