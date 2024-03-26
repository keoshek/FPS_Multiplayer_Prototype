using Unity.Netcode;
using UnityEngine;

public class AnimationController : NetworkBehaviour
{
    private Animator _animator;

    [HideInInspector] public enum AnimationState { 
        Idle,
        Run,
        RunReverse
    }
    [HideInInspector] public AnimationState _state;

    private void Start()
    {
        if (!IsOwner) return;

        _animator = GetComponent<Animator>();
    }

    public void ChangeState(AnimationState _state)
    {
        if (!IsOwner) return;

        if (this._state == _state) return;

        switch (_state) {
            case AnimationState.Idle:
                _animator.SetFloat("Run", 0f);
                break;
            case AnimationState.Run:
                _animator.SetFloat("Run", 1f);
                break;
            case AnimationState.RunReverse:
                _animator.SetFloat("Run", -1f);
                break;
        }
        this._state = _state;
    }
}
