using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyUI : MonoBehaviour
{
    private SlideUI _readySlideUI;

    private void Awake() {
        _readySlideUI = GetComponent<SlideUI>();
    }

    private void OnEnable() {
        PlayerController.OnReady += PlayerController_OnReady;
    }

    private void OnDisable() {
        PlayerController.OnReady -= PlayerController_OnReady;
    }

    private void PlayerController_OnReady() {
        StartCoroutine(_readySlideUI.SlideInRoutine());
    }
}
