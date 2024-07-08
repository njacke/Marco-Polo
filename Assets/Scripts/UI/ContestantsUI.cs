using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContestantsUI : MonoBehaviour
{
    [SerializeField] private Image _itaDisplay;
    [SerializeField] private Image _gerDisplay;
    [SerializeField] private Image _spaDisplay;
    [SerializeField] private Image _usaDisplay;
    [SerializeField] private Sprite[] _itaSprites;
    [SerializeField] private Sprite[] _gerSprites;
    [SerializeField] private Sprite[] _spaSprites;
    [SerializeField] private Sprite[] _usaSprites;
    private Dictionary<NPC.NPCType, Image> _npcDisplayDict;
    private Dictionary<NPC.NPCType, Sprite[]> _npcSpritesDict;

    private void Awake() {
        Init();        
    }

    private void OnEnable() {
        GameManager.OnLevelLoaded += GameManager_OnLevelLoaded;
        GuessUI.OnGuessResult += GuessUI_OnGuessResult;
    }

    private void OnDisable() {
        GameManager.OnLevelLoaded -= GameManager_OnLevelLoaded;
        GuessUI.OnGuessResult -= GuessUI_OnGuessResult;
    }

    private void Init() {
        _npcDisplayDict = new Dictionary<NPC.NPCType, Image>() {
            { NPC.NPCType.Italian, _itaDisplay },
            { NPC.NPCType.German, _gerDisplay },
            { NPC.NPCType.Spaniard, _spaDisplay },
            { NPC.NPCType.American, _usaDisplay },
        };

        _npcSpritesDict = new Dictionary<NPC.NPCType, Sprite[]>() {
            { NPC.NPCType.Italian, _itaSprites },
            { NPC.NPCType.German, _gerSprites },
            { NPC.NPCType.Spaniard, _spaSprites },
            { NPC.NPCType.American, _usaSprites },
        };

        foreach (var display in _npcDisplayDict) {
            display.Value.gameObject.SetActive(false);
        }
    }

    private void GameManager_OnLevelLoaded(GameManager.Level level) {
        NPC[] currentNPCs = GameManager.Instance.GetActiveNPCs;
        foreach (var npc in currentNPCs) {
            if (npc != null && npc.IsContestant) {
                if (_npcDisplayDict.TryGetValue(npc.GetNPCType, out var display)) {
                    display.sprite = _npcSpritesDict[npc.GetNPCType][0];
                    display.gameObject.SetActive(true);
                }
            } 
        }        
    }

    private void GuessUI_OnGuessResult(bool isAnswerCorrect) {
        if (isAnswerCorrect) {
            var npcType = GameManager.Instance.CurrentCaughtNPC.GetNPCType;
            _npcDisplayDict[npcType].sprite = _npcSpritesDict[npcType][1];
        }
    }
}
