using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameUI : MonoBehaviour
{
    [SerializeField] private float _loadTutorialDelay = .5f; 

    public void OnMenuSelected() {
        StartCoroutine(LoadMainMenuRoutine());
    }

    private IEnumerator LoadMainMenuRoutine() {
        yield return new WaitForSecondsRealtime(_loadTutorialDelay);
        SceneLoader.LoadSceneAsync(SceneLoader.SCENE_MAIN_MENU_STRING);
    }
}
