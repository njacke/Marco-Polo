using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static Action OnScan;
    
    [SerializeField] private float _baseMoveSpeed = 100f;

    private Rigidbody2D _rb;
    private PlayerInput  _playerInput;
    private FrameInput _frameInput;
    
    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Update() {
        GatherInput();
        Move();
        HandleScan();
    }

    private void FixedUpdate() {
        Move();
    }

    private void GatherInput() {
        _frameInput = _playerInput.FrameInput;
    }

    private void Move() {
        Vector2 movement = _baseMoveSpeed * Time.fixedDeltaTime * _frameInput.Move;
        //Debug.Log("New movement: " + movement);
        _rb.velocity = movement;
    }

    private void HandleScan() {
        if (_frameInput.Scan) {
            Debug.Log("Scan skill has been used.");
            OnScan?.Invoke();
        }
    }

    private void Cheat() {

    }
}
