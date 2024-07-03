using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private float _cursorDisplayDelay = 0f;
    [SerializeField] private Sprite _cursorDefaultSprite;
    [SerializeField] private Sprite _cursorClickSprite;
    private Image image;

    private void Awake() {
        image = GetComponent<Image>();
        image.sprite = _cursorDefaultSprite;
        image.enabled = false;
    }

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;     

        StartCoroutine(CursorDisplayAndUnlockRoutine());
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

    private IEnumerator CursorDisplayAndUnlockRoutine() {
        yield return new WaitForSecondsRealtime(_cursorDisplayDelay);
        
        //confine cursor if not in Unity
        if (Application.isPlaying) {
            Cursor.lockState = CursorLockMode.None;
        } else {
            Cursor.lockState = CursorLockMode.Confined;
        }

        image.enabled = true;
    }
}