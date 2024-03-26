using UnityEngine;

public class InputManager : MonoBehaviour
{
    [HideInInspector] public InputMaster _inputMaster;

    private void Awake()
    {
        _inputMaster = new InputMaster();
    }

    private void OnEnable()
    {
        _inputMaster.Enable();
    }

    private void OnDisable()
    {
        _inputMaster.Disable();
    }

    public Vector2 GetPlayerMovement()
    {
        // return vector2
        return _inputMaster.Player.Movement.ReadValue<Vector2>();
    }

    public Vector2 GetMouseDelta()
    {
        // return vector2
        return _inputMaster.Player.Look.ReadValue<Vector2>();
    }

    public bool PlayerJumped()
    {
        // return true if pressed at that frame, else return false
        return _inputMaster.Player.Jump.triggered;
    }

    public float PlayerIsRunning()
    {
        // return 1 if runs, else return 0
        return _inputMaster.Player.Run.ReadValue<float>();
    }
}
