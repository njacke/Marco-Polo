using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private ObstacleType _obstacleType;
    [SerializeField] private float _triggerWaveCD = 1f;
    private Rigidbody2D _rb;
    private SoundWave _soundWave;
    private float _triggerWaveCDRemaning = 0f;

    public enum ObstacleType {
        None,
        Duck
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
            }
        }
    }

    private void GameManager_OnGameStarted() {
        _rb.velocity = Vector2.zero;
    }
}
