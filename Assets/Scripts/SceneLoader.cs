using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static Task LoadSceneAsync(string sceneName)
    {
        var tcs = new TaskCompletionSource<bool>();
        var asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        asyncOperation.completed += (AsyncOperation op) =>
            { tcs.SetResult(true); };

        Debug.Log(sceneName + " scene loaded!");
        return tcs.Task;
    }
}
