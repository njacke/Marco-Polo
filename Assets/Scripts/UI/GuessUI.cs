using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GuessUI : MonoBehaviour
{
    public static Action OnCorrectAnswerSelected;

    [SerializeField] private Button[] _answersButtons;
    [SerializeField] private Image _imageDisplayed;
    [SerializeField] private Sprite[] _italianSprites;
    [SerializeField] private Sprite[] _germanSprites;
    [SerializeField] private Sprite[] _spaniardSprites;
    [SerializeField] private Sprite[] _americanSprites;
    
    private NPC.NPCType _currentNPCType;
    private Dictionary<NPC.NPCType, Sprite[]> _npcTypeSpriteDict;
    private Dictionary<NPC.NPCType, int> _npcTypeIndexDict;

    private void Awake() {
        Init();
    }

    private void OnEnable() {
        if (GameManager.Instance != null) {
            _currentNPCType = GameManager.Instance.CurrentCaughtNPC.GetNPCType;
            _imageDisplayed.sprite = _npcTypeSpriteDict[_currentNPCType][0];
        }
    }

    private void Init() {    
        _npcTypeSpriteDict = new Dictionary<NPC.NPCType, Sprite[]>() {
            { NPC.NPCType.Italian, _italianSprites },
            { NPC.NPCType.German, _germanSprites },
            { NPC.NPCType.Spaniard, _spaniardSprites },
            { NPC.NPCType.American, _americanSprites },
        };

        _npcTypeIndexDict = new Dictionary<NPC.NPCType, int>() {
            { NPC.NPCType.Italian, 0 },
            { NPC.NPCType.German, 1 },
            { NPC.NPCType.Spaniard, 2 },
            { NPC.NPCType.American, 3 },
        };
    }

    private IEnumerator RevealImageRoutine() {
        var slideUI = _imageDisplayed.GetComponent<SlideUI>();

        yield return StartCoroutine(slideUI.SlideOutRoutine());
        _imageDisplayed.sprite = _npcTypeSpriteDict[_currentNPCType][1];        
        yield return StartCoroutine(slideUI.SlideInRoutine());        
    }

    public void OnAnswerSelected (int index) {

        // set buttons to inactive except for selected answer        
        for (int i = 0; i < _answersButtons.Length; i++) {
            if (i == index) continue;

            _answersButtons[i].interactable = false;
        }

        if (_npcTypeIndexDict.TryGetValue(_currentNPCType, out int correctIndex)) {
            if (index == correctIndex) {
                Debug.Log("Answer was correct");
                // TODO: yield return
                StartCoroutine(RevealImageRoutine());
                // display result image
            } else {
                Debug.Log("Answer was incorrect");
                // incorrect answer logic
            }
        } else {
            Debug.Log("Couldn't get index value from dict");
        }
    }
}
