using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class NPC : MonoBehaviour
{    
    [SerializeField] private float _baseMoveSpeed = 100f;
    [SerializeField] private float _moveDirChangeTime = 2f;
    
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;

    private bool _isCaught = false;
    private Vector2 _currentMoveDir = Vector2.zero;
    private float _timeMoving = 0f;

    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.enabled = false;
    }

    private void Start() {
        _currentMoveDir = GetMoveDir();
    }

    private void OnEnable() {
        PlayerController.OnScan += ScanResponse;
    }

    private void OnDisable() {
        PlayerController.OnScan -= ScanResponse;
    }

    private void Update() {
        _timeMoving += Time.deltaTime;
        if (_timeMoving > _moveDirChangeTime) {
            _currentMoveDir = GetMoveDir();
            _timeMoving = 0f;
        }
    }

    private void FixedUpdate() {
        Move();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.GetComponent<CatchDetector>()) {
            Debug.Log("I have been caught.");
            _isCaught = true;
            Destroy(this.gameObject);
            //Time.timeScale = 0f;
        }
    }

    private void Move() {
        Vector2 movement = _baseMoveSpeed * Time.fixedDeltaTime * _currentMoveDir;
        _rb.velocity = movement;
    }

    private Vector2 GetMoveDir() {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    private void ScanResponse() {
        Debug.Log("Scan response triggered!");
        StartCoroutine(ScanResponseRoutine());
    }

    private IEnumerator ScanResponseRoutine() {
        _spriteRenderer.enabled = true;

        yield return new WaitForSeconds(2f);

        _spriteRenderer.enabled = false;
    }
}
