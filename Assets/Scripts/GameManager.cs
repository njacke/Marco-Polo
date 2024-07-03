using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static Action OnGamePaused;
    
    public static GameManager Instance;

    [SerializeField] private GameObject _guessUI;
    [SerializeField] private float _guessUIDisplayDelay = 2f;

    public NPC CurrentCaughtNPC { get; private set; } = null;

    private void Awake() {
        if (Instance == null) { Instance = this; }

        _guessUI.SetActive(false);
    }

    private void OnEnable() {
        NPC.OnCaughtNPC += NPC_HandleNPCCaught;
        GuessUI.OnCorrectAnswerSelected += GuessUI_HandleCorrectAnswer;
    }

    private void OnDisable() {
        NPC.OnCaughtNPC -= NPC_HandleNPCCaught;        
        GuessUI.OnCorrectAnswerSelected -= GuessUI_HandleCorrectAnswer;
    }

    private void NPC_HandleNPCCaught(NPC npc) {
        StartCoroutine(HandleNPCCaughtRoutine(npc));
    }

    private IEnumerator HandleNPCCaughtRoutine(NPC npc) {
        Time.timeScale = 0;
        OnGamePaused?.Invoke();

        CurrentCaughtNPC = npc;
        
        yield return new WaitForSecondsRealtime(_guessUIDisplayDelay);
        _guessUI.SetActive(true);        
    }

    private void GuessUI_HandleCorrectAnswer() {
        CurrentCaughtNPC.gameObject.SetActive(false);
    }
}
