using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class SoundWave : MonoBehaviour
{
    private Animator _myAnim;

    private readonly int SOUND_WAVE_HASH = Animator.StringToHash("Base Layer.SoundWave");
    private readonly int WAVE_START_HASH = Animator.StringToHash("WaveStart");
    private readonly int WAVE_END_HASH = Animator.StringToHash("WaveEnd");


    private void Awake() {
        _myAnim = GetComponent<Animator>();
    }

    private bool IsAnimInProgress() {
        AnimatorStateInfo stateInfo = _myAnim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.fullPathHash == SOUND_WAVE_HASH;
    }

    private void EndAnimEvent() {
        _myAnim.SetTrigger(WAVE_END_HASH);
    }
    
    public void TriggerSoundWave() {
        if (!IsAnimInProgress()) {
            _myAnim.SetTrigger(WAVE_START_HASH);
        }
    }

    public void ResetSoundWave() {
        _myAnim.enabled = false;
        _myAnim.enabled = true;
    }
}
