using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using Unity.VisualScripting;

public class GuessUI : MonoBehaviour
{
    public static Action<bool> OnGuessAnswer;

    [SerializeField] private float _resetDelay = 1f;
    [SerializeField] private Button[] _answersButtons;
    [SerializeField] private Image _titleImage;
    [SerializeField] private Image _npcImage;
    [SerializeField] private Image _resultImage;
    [SerializeField] private Sprite[] _resultSprites;
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
        GameManager.OnGuess += GameManager_OnGuess;
    }

    private void OnDisable() {
        GameManager.OnGuess -= GameManager_OnGuess;
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


    private void GameManager_OnGuess(NPC.NPCType type) {
        _currentNPCType = type;
        StartCoroutine(LoadGuessUIRoutine(type));
    }

    private IEnumerator LoadGuessUIRoutine(NPC.NPCType type) {
        _npcImage.sprite = _npcTypeSpriteDict[type][0];
        var npcSlideUI = _npcImage.GetComponent<SlideUI>();
        yield return npcSlideUI.SlideInRoutine();

        var titleSlideUI = _titleImage.GetComponent<SlideUI>();
        yield return titleSlideUI.SlideInRoutine();
    }

    private IEnumerator ResetGuessUIRoutine(bool isCorrectAnswer) {
        _resultImage.sprite = isCorrectAnswer ? _resultSprites[0] : _resultSprites[1];
        var resultSlideUI = _resultImage.GetComponent<SlideUI>();
        yield return resultSlideUI.SlideInRoutine();

        var npcSlideUI = _npcImage.GetComponent<SlideUI>();
        yield return npcSlideUI.SlideOutRoutine();

        var titleSlideUI = _titleImage.GetComponent<SlideUI>();
        yield return titleSlideUI.SlideOutRoutine();

        yield return resultSlideUI.SlideOutRoutine();

        foreach (var button in _answersButtons) {
            button.interactable = true;
            if (EventSystem.current.currentSelectedGameObject == button.gameObject) {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }

    private IEnumerator RevealImageRoutine() {
        var npcSlideUI = _npcImage.GetComponent<SlideUI>();
        yield return npcSlideUI.SlideOutRoutine();
        _npcImage.sprite = _npcTypeSpriteDict[_currentNPCType][1];        
        yield return npcSlideUI.SlideInRoutine();        
    }


    public IEnumerator OnAnswerSelectedRoutine(int index) {
        // set buttons to inactive except for selected answer        
        for (int i = 0; i < _answersButtons.Length; i++) {
            if (i == index) continue;

            _answersButtons[i].interactable = false;
        }

        if (_npcTypeIndexDict.TryGetValue(_currentNPCType, out int correctIndex)) {
            if (index == correctIndex) {
                Debug.Log("Answer was correct");
                // TODO: yield return
                yield return RevealImageRoutine();
                yield return new WaitForSecondsRealtime(_resetDelay);
                yield return ResetGuessUIRoutine(true);
                OnGuessAnswer?.Invoke(true);
            } else {
                yield return ResetGuessUIRoutine(false);
                OnGuessAnswer?.Invoke(false);
            }
        } else {
            Debug.Log("Couldn't get index value from dict");
        }
    }

    public void OnAnswerSelected(int index) {
        StartCoroutine(OnAnswerSelectedRoutine(index));
    }
}
