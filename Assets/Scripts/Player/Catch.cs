using UnityEngine;

public class Catch : MonoBehaviour
{
    private Animator _myAnim;
    private Collider2D _myCollider;

    private readonly int ENTRY_HASH = Animator.StringToHash("Base Layer.Entry");
    private readonly int CATCH_HASH = Animator.StringToHash("Base Layer.Catch");
    private readonly int CATCH_START_HASH = Animator.StringToHash("CatchStart");
    private readonly int CATCH_END_HASH = Animator.StringToHash("CatchEnd");


    private void Awake() {
        _myAnim = GetComponent<Animator>();
        _myCollider = GetComponentInChildren<Collider2D>();
        _myCollider.enabled = false;
    }

    private bool IsAnimInProgress() {
        AnimatorStateInfo stateInfo = _myAnim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.fullPathHash == CATCH_HASH;
    }

    private void MidAnimEvent() {
        _myCollider.enabled = true;
    }

    private void EndAnimEvent() {
        _myAnim.SetTrigger(CATCH_END_HASH);
        _myCollider.enabled = false;
    }
    
    public void TriggerCatch() {
        if (!IsAnimInProgress()) {
            _myAnim.SetTrigger(CATCH_START_HASH);
        }
    }

    public void ResetCatch() {
        _myAnim.ResetTrigger(CATCH_START_HASH);
        _myAnim.ResetTrigger(CATCH_END_HASH);
        _myAnim.Play(ENTRY_HASH, 0, 0.0f);
        _myCollider.enabled = false;
    }
}
