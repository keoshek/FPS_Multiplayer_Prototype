using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerControl : NetworkBehaviour
{
    private AnimationController _animationController;
    private InputManager _inputManager;
    private CinemachineVirtualCamera _virtualCamera;
    private Rigidbody _rb;
    private float _xRotation = 0;
    private Vector3 _networkPosition = new();
    private Quaternion _networkRotation = new();

    [SerializeField] private float _speed = 10;
    [SerializeField] private float _runSpeed = 15;
    [SerializeField] private float _jumpForce = 200;
    [SerializeField] private float _mouseSensitivity = 25;
    [SerializeField] private bool _isGrounded;
    [SerializeField] private Transform _cameraRoot;
    [SerializeField] private Vector2 _defaultInitialPosition = new Vector2(-4, 4);
    [SerializeField] private Vector2 _verticalLookClampValues;

    private void Start()
    {
        if (IsOwner) {
            Initialize();
        }
    }

    private void Update()
    {
        if (IsOwner) {
            Move();
            Look();
            SendPositionRotationServerRpc(transform.position, transform.rotation);
        } else {
            transform.SetPositionAndRotation(_networkPosition, _networkRotation);
        }
    }

    private void Initialize()
    {
        // set up components
        if (_animationController == null) _animationController = GetComponent<AnimationController>();
        if (_inputManager == null) _inputManager = GetComponent<InputManager>();
        if (_rb == null) _rb = GetComponent<Rigidbody>();
        if (_virtualCamera == null) _virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

        // add jump method to jump button press event
        _inputManager._inputMaster.Player.Jump.started += _ => Jump();

        // spawn in random position
        transform.position = new Vector3(Random.Range(_defaultInitialPosition.x, _defaultInitialPosition.y), 0, Random.Range(_defaultInitialPosition.x, _defaultInitialPosition.y));

        // attach camera to root
        _virtualCamera.Follow = _cameraRoot;

        Cursor.visible = false;
    }

    private void Move()
    {
        Vector2 _movementInput = _inputManager.GetPlayerMovement();
        Vector3 _move = new Vector3(_movementInput.x, 0, _movementInput.y);
        // open below code for charactercontrol.move
        //_move = transform.forward * _move.z + transform.right * _move.x;
        //_move.y = 0;

        if (_move == Vector3.zero) {
            _animationController.ChangeState(AnimationController.AnimationState.Idle);
            return;
        }

        float _runInput = _inputManager.PlayerIsRunning();
        _move *= _runInput == 0 ? _speed : _runSpeed;
        //_characterController.Move(_move * Time.deltaTime);
        transform.Translate(_move * Time.deltaTime);

        if (_movementInput.y >= 0) _animationController.ChangeState(AnimationController.AnimationState.Run);
        else _animationController.ChangeState(AnimationController.AnimationState.RunReverse);
    }

    private void Look()
    {
        Vector2 _mouseInput = _inputManager.GetMouseDelta();
        _mouseInput *= _mouseSensitivity * Time.deltaTime;

        _xRotation -= _mouseInput.y;
        _xRotation = Mathf.Clamp(_xRotation, _verticalLookClampValues.x, _verticalLookClampValues.y);

        _cameraRoot.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        transform.Rotate(Vector3.up * _mouseInput.x);
    }

    private void Jump()
    {
        if (_isGrounded) {
            _rb.AddForce(Vector3.up * _jumpForce);
        }
    }

    [ServerRpc]
    public void SendPositionRotationServerRpc(Vector3 _position, Quaternion _rotation)
    {
        UpdatePositionRotationClientRpc(_position, _rotation);
    }

    [ClientRpc]
    public void UpdatePositionRotationClientRpc(Vector3 _position, Quaternion _rotation)
    {
        if (IsOwner) return;

        _networkPosition = _position;
        _networkRotation = _rotation;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!IsOwner) return;

        if (other.transform.CompareTag("Ground")) {
            _isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (!IsOwner) return;

        if (other.transform.CompareTag("Ground")) {
            _isGrounded = false;
        }
    }
}
