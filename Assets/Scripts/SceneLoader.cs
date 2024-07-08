using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public const string SCENE_MAIN_MENU_STRING = "Main Menu";
    public const string SCENE_TUTORIAL_STRING = "Tutorial";
    public const string SCENE_GAME_STRING = "Game";
    public const string SCENE_END_MENU_STRING = "End Menu";

    public static void LoadScene(string sceneName) {
         SceneManager.LoadScene(sceneName);
    }

    public static Task LoadSceneAsync(string sceneName) {
        var tcs = new TaskCompletionSource<bool>();
        var asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        asyncOperation.completed += (AsyncOperation op) =>
            { tcs.SetResult(true); };

        Debug.Log(sceneName + " scene loaded!");
        return tcs.Task;
    }
}
