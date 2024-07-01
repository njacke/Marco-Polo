using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public FrameInput FrameInput { get; private set; }
    private PlayerInputActions _playerInputAcitons;
    private InputAction _move, _scan, _cheat;

    private void Awake() {
        _playerInputAcitons = new PlayerInputActions();

        _move = _playerInputAcitons.Player.Move;
        _scan = _playerInputAcitons.Player.Scan;
        _cheat = _playerInputAcitons.Player.Cheat;
    }

    private void OnEnable() {
        _playerInputAcitons.Enable();
    }

    private void OnDisable() {
        _playerInputAcitons.Disable();
    }

    private void Update() {
        FrameInput = GatherInput();
    }

    private FrameInput GatherInput() {
        return new FrameInput {
            Move = _move.ReadValue<Vector2>(),
            Scan = _scan.WasPressedThisFrame(),
            Cheat = _cheat.WasPressedThisFrame(),
        };
    }
}

public struct FrameInput {
    public Vector2 Move;
    public bool Scan;
    public bool Cheat;
}
