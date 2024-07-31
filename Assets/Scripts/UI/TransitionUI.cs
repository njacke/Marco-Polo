using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TransitionUI : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;
    [SerializeField] private float _fadeInSpeed = .3f;
    [SerializeField] private float _fadeOutSpeed = 1f;
    [SerializeField] private float _fadeDelay = .5f;
    [SerializeField] private bool _fadeOutOnStart = true;

    private void Start() {
        if (_fadeOutOnStart) {
            FadeOut();
        }        
    }

    private void FadeOut() {
        StartCoroutine(FadeRoutine(0f, _fadeOutSpeed, true));
    }

    public IEnumerator FadeInRoutine() {
        yield return FadeRoutine(1f, _fadeInSpeed, false);
    }

    private IEnumerator FadeRoutine(float targetAlpha, float fadeSpeed, bool isDelayed){
        if (isDelayed) {
            yield return new WaitForSeconds(_fadeDelay);
        }

        while(!Mathf.Approximately(_fadeImage.color.a, targetAlpha)) {
            float alpha = Mathf.MoveTowards(_fadeImage.color.a, targetAlpha, fadeSpeed * Time.unscaledDeltaTime);
            _fadeImage.color = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, alpha);
            yield return null;
        }
        
        Debug.Log("Routine finished with target alpha " + targetAlpha);
    }
}
