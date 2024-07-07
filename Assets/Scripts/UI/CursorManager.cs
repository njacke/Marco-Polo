using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Sprite _cursorDefaultSprite;
    [SerializeField] private Sprite _cursorClickSprite;
    [SerializeField] private bool _enableOnStart = false;
    private Image image;

    private void Awake() {
        image = GetComponent<Image>();
        image.sprite = _cursorDefaultSprite;
    }

    private void Start() {
        DisableCursor(); 

        if (_enableOnStart) {
            DisplayAndEnableCursor();
        }
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            image.sprite = _cursorClickSprite;

        } else if (Input.GetMouseButtonUp(0)) {
            image.sprite = _cursorDefaultSprite;
        }

        Vector2 cursorPos = Input.mousePosition;
        image.rectTransform.position = cursorPos;

        if (!Application.isPlaying) { return; }

        Cursor.visible = false;
    }

    public void DisableCursor() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;     
        image.enabled = false;
    }

    public void DisplayAndEnableCursor() {
        Debug.Log("Cursor display called");
        
        //confine cursor if not in Unity
        if (Application.isPlaying) {
            Cursor.lockState = CursorLockMode.None;
        } else {
            Cursor.lockState = CursorLockMode.Confined;
        }

        image.enabled = true;
    }
}