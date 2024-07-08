using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class SoundWave : MonoBehaviour
{
    private Animator _myAnim;

    private readonly int ENTRY_HASH = Animator.StringToHash("Base Layer.Entry");
    private readonly int SOUND_WAVE_HASH = Animator.StringToHash("Base Layer.SoundWave");
    private readonly int WAVE_START_HASH = Animator.StringToHash("WaveStart");
    private readonly int WAVE_END_HASH = Animator.StringToHash("WaveEnd");


    private void Awake() {
        _myAnim = GetComponent<Animator>();
    }

    public bool IsAnimInProgress() {
        if (_myAnim != null) {
            AnimatorStateInfo stateInfo = _myAnim.GetCurrentAnimatorStateInfo(0);
            return stateInfo.fullPathHash == SOUND_WAVE_HASH;
        }

        return true;
    }

    private void EndAnimEvent() {
        _myAnim.SetTrigger(WAVE_END_HASH);
    }
    
    public bool TriggerSoundWave() {
        Debug.Log("Sound wave triggered.");
        if (!IsAnimInProgress()) {
            Debug.Log("Setting trigger");
            _myAnim.SetTrigger(WAVE_START_HASH);
            return true;
        }
        return false;
    }

    public void ResetSoundWave() {
        _myAnim.ResetTrigger(WAVE_START_HASH);
        _myAnim.ResetTrigger(WAVE_END_HASH);
        _myAnim.Play(ENTRY_HASH, 0, 0.0f);
    }
}
