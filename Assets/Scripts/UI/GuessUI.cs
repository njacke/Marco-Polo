using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class GuessUI : MonoBehaviour
{
    public static Action<bool> OnGuessResult;
    public static Action<bool> OnGuessEnd;

    [SerializeField] private float _resetDelay = 1f;
    [SerializeField] private GameObject _answersButtons;
    [SerializeField] private Button[] _answersButtonsArray;
    [SerializeField] private Image _titleImage;
    [SerializeField] private Image _npcImage;
    [SerializeField] private Image _resultImage;
    [SerializeField] private GameObject _answerButtons;
    [SerializeField] private Sprite[] _resultSprites;
    [SerializeField] private Sprite[] _italianSprites;
    [SerializeField] private Sprite[] _germanSprites;
    [SerializeField] private Sprite[] _spaniardSprites;
    [SerializeField] private Sprite[] _americanSprites;
    
    private NPC.NPCType _currentNPCType;
    private CursorManager _cursorManager;
    private Dictionary<NPC.NPCType, Sprite[]> _npcTypeSpriteDict;
    private Dictionary<NPC.NPCType, int> _npcTypeIndexDict;

    private void Awake() {
        Init();
    }

    private void OnEnable() {
        GameManager.OnGuess += GameManager_OnGuess;

        TutorialPopUpsUI.OnCatchTutorialDone += TutorialPopUpsUI_OnCatchTutorialDone;
    }

    private void OnDisable() {
        GameManager.OnGuess -= GameManager_OnGuess;

        TutorialPopUpsUI.OnCatchTutorialDone -= TutorialPopUpsUI_OnCatchTutorialDone;
    }

    private void Init() {    
        _cursorManager = GetComponentInChildren<CursorManager>();

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

    private void TutorialPopUpsUI_OnCatchTutorialDone() {
        StartCoroutine(LoadGuessUIRoutine(TutorialManager.Instance.GetTutorialNPC.GetNPCType));
    }


    private void GameManager_OnGuess(NPC.NPCType type) {
        if (TutorialManager.Instance != null) {
            _currentNPCType = TutorialManager.Instance.GetTutorialNPC.GetNPCType;
        } else {
            _currentNPCType = type;
        }

        StartCoroutine(LoadGuessUIRoutine(type));
    }

    private IEnumerator LoadGuessUIRoutine(NPC.NPCType type) {

        if (TutorialManager.Instance != null) {
            _answersButtonsArray[1].interactable = false;
            _answersButtonsArray[2].interactable = false;
            _answersButtonsArray[3].interactable = false;
        }

        else if (GameManager.Instance != null) {
            switch (GameManager.Instance.GetActiveLevel) {
                case GameManager.Level.Pool:
                _answersButtonsArray[2].interactable = false;
                _answersButtonsArray[3].interactable = false;
                break;
                case GameManager.Level.Garden:
                _answersButtonsArray[3].interactable = false;
                break;
                default:
                break;
            }
        }

        _npcImage.sprite = _npcTypeSpriteDict[type][0];
        var npcSlideUI = _npcImage.GetComponent<SlideUI>();
        yield return npcSlideUI.SlideInRoutine();

        var titleSlideUI = _titleImage.GetComponent<SlideUI>();
        yield return titleSlideUI.SlideInRoutine();

        var buttonsSlideUI = _answerButtons.GetComponent<SlideUI>();
        yield return buttonsSlideUI.SlideInRoutine();

        _cursorManager.gameObject.SetActive(true);
        _cursorManager.DisplayAndEnableCursor();
    }

    private IEnumerator ResetGuessUIRoutine(bool isCorrectAnswer) {
        _resultImage.sprite = isCorrectAnswer ? _resultSprites[0] : _resultSprites[1];
        var resultSlideUI = _resultImage.GetComponent<SlideUI>();
        yield return resultSlideUI.SlideInRoutine();
        OnGuessResult?.Invoke(isCorrectAnswer);

        yield return new WaitForSecondsRealtime(_resetDelay);
        _cursorManager.DisableCursor();
        _cursorManager.gameObject.SetActive(false);

        var npcSlideUI = _npcImage.GetComponent<SlideUI>();
        StartCoroutine(npcSlideUI.SlideOutRoutine());

        var titleSlideUI = _titleImage.GetComponent<SlideUI>();
        StartCoroutine(titleSlideUI.SlideOutRoutine());

        var buttonsSlideUI = _answerButtons.GetComponent<SlideUI>();
        StartCoroutine(buttonsSlideUI.SlideOutRoutine());

        yield return resultSlideUI.SlideOutRoutine();

        foreach (var button in _answersButtonsArray) {
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
        for (int i = 0; i < _answersButtonsArray.Length; i++) {
            if (i == index) continue;

            _answersButtonsArray[i].interactable = false;
        }

        if (TutorialManager.Instance != null) {
            Debug.Log("Setting NPC type for tutorial");
            _currentNPCType = TutorialManager.Instance.GetTutorialNPC.GetNPCType;
        }

        if (_npcTypeIndexDict.TryGetValue(_currentNPCType, out int correctIndex)) {
            if (index == correctIndex) {
                Debug.Log("Answer was correct");
                yield return RevealImageRoutine();
                yield return ResetGuessUIRoutine(true);
                OnGuessEnd?.Invoke(true);
            } else {
                yield return ResetGuessUIRoutine(false);
                OnGuessEnd?.Invoke(false);
            }
        } else {
            Debug.Log("Couldn't get index value from dict");
        }
    }

    public void OnAnswerSelected(int index) {
        StartCoroutine(OnAnswerSelectedRoutine(index));
    }
}
