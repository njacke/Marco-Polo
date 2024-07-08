using UnityEngine;
using System;

public class Obstacle : MonoBehaviour
{
    public static Action<Obstacle> OnWaveTriggered;

    [SerializeField] private ObstacleType _obstacleType;
    [SerializeField] private float _triggerWaveCD = 1f;
    private Rigidbody2D _rb;
    private SoundWave _soundWave;
    private float _triggerWaveCDRemaning = 0f;

    public ObstacleType GetObstacleType { get => _obstacleType; }

    public enum ObstacleType {
        None,
        Duck,
        Tube,
        Fontain,
        Table,
        Piano,
        Trolley
    }

    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _soundWave = GetComponentInChildren<SoundWave>();
    }

    private void Update() {
        _triggerWaveCDRemaning -= Time.deltaTime;
    }

    private void OnEnable() {
        GameManager.OnGameStarted += GameManager_OnGameStarted;
    }

    private void OnDisable() {
        GameManager.OnGameStarted -= GameManager_OnGameStarted;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        Debug.Log("Obstacle was hit");
        if (other.gameObject.GetComponent<PlayerController>() || other.gameObject.GetComponent<NPC>() || other.gameObject.GetComponent<Obstacle>()) {
            if (_triggerWaveCDRemaning <= 0f) {
                _soundWave.TriggerSoundWave();
                _triggerWaveCDRemaning = _triggerWaveCD;
                OnWaveTriggered?.Invoke(this);
            }
        }
    }

    private void GameManager_OnGameStarted() {
        _rb.velocity = Vector2.zero;
        _soundWave.ResetSoundWave();
    }
}
