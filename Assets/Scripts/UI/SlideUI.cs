using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideUI : MonoBehaviour
{
    [SerializeField] private Vector3 _targetPos;
    [SerializeField] private float _slideDuration = 1f;
    [SerializeField] private float _slideDelay = 1f;
    [SerializeField] private bool _slideInOnEnable = true;
    private RectTransform _rectTransform;
    private Vector3 _awakePos;

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
        _awakePos = _rectTransform.localPosition;
    }

    private void OnEnable() {
        if (_slideInOnEnable) StartCoroutine(SlideInRoutine());
    }

    public IEnumerator SlideInRoutine() {
        yield return StartCoroutine(SlideToTargetRoutine(_targetPos));
    }

    public IEnumerator SlideOutRoutine() {
        yield return StartCoroutine(SlideToTargetRoutine(_awakePos));
    }

    private IEnumerator SlideToTargetRoutine(Vector3 targetPos) {
        yield return new WaitForSecondsRealtime(_slideDelay);
        
        Vector3 startPos = _rectTransform.localPosition;    
        float timePassed = 0f;    

        while (_rectTransform.localPosition != targetPos) {
            timePassed += Time.unscaledDeltaTime;
            var linearT = timePassed / _slideDuration;
            Vector3 newLocalPos = Vector3.Lerp(startPos, targetPos, linearT);

            _rectTransform.localPosition = newLocalPos;

            yield return null;
        }
    }
}
