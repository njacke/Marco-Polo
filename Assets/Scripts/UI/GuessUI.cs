using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GuessUI : MonoBehaviour
{
    [SerializeField] private Image _imageDisplayed;
    [SerializeField] private Button _button0;
    [SerializeField] private Sprite[] _boy1Sprites;
    [SerializeField] private Sprite[] _boy2Sprites;
    [SerializeField] private Sprite[] _girl1Sprites;
    [SerializeField] private Sprite[] _girl2Sprites;
    
    private NPC.NPCType _currentNPC;
    private Dictionary<NPC.NPCType, int> _npcTypeIndexDict;

    private void OnEnable() {
        Init();
    }

    private void Init() {
        _npcTypeIndexDict = new Dictionary<NPC.NPCType, int>() {
            { NPC.NPCType.Boy1, 0 },
            { NPC.NPCType.Boy2, 1 },
            { NPC.NPCType.Girl1, 2 },
            { NPC.NPCType.Girl2, 3 },
        };

        _currentNPC = GameManager.Instance.CurrentCaughtNPC;
    }

    public void OnAnswerSelected (int index) {
        if (_npcTypeIndexDict.TryGetValue(_currentNPC, out int correctIndex)) {
            if (index == _npcTypeIndexDict[_currentNPC]) {
                Debug.Log("Answer was correct");
                // correct answer logic
            } else {
                Debug.Log("Answer was incorrect");
                // incorrect answer logic
            }
        } else {
            Debug.Log("Couldn't get index value from dict");
        }
    }
}
