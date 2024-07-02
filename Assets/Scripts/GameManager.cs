using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static Action OnGamePaused;
    
    public static GameManager Instance;

    [SerializeField] private GameObject _guessUI;

    public NPC.NPCType CurrentCaughtNPC { get; private set; } = NPC.NPCType.None;

    private void Awake() {
        if (Instance == null) { Instance = this; }
    }

    private void OnEnable() {
        NPC.OnCaughtNPC += HandleNPCCaught;
    }

    private void OnDisable() {
        NPC.OnCaughtNPC -= HandleNPCCaught;        
    }

    private void HandleNPCCaught(NPC.NPCType npcType) {
        Time.timeScale = 0;
        CurrentCaughtNPC = npcType;
        _guessUI.SetActive(true);

        OnGamePaused?.Invoke();
    }
}
