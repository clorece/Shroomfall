using UnityEngine;
using Fusion;

public class PlayerMovement : NetworkBehaviour
{
    // setup
    private NetworkCharacterController _ncc;
    [SerializeField] private Transform _visualsRoot;

    [Header("Camera Control")]
    [SerializeField] private Transform _cameraHolder;
    [SerializeField] private float _mouseSensitivity = 2.0f;

    private float _pitch = 0.0f;
    private float _yaw = 0.0f;

    public override void Spawned()
    {
        _ncc = GetComponent<NetworkCharacterController>();

        if (Object.HasInputAuthority)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _yaw = transform.rotation.eulerAngles.y;
        }
    }

    // Update is for local visuals
    void Update()
    {
        if (Object.HasInputAuthority)
        {
            // Get mouse input
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

            // Rotate the visuals object left/right
            _yaw += mouseX;
            _visualsRoot.rotation = Quaternion.Euler(0, _yaw, 0);

            // Rotate the camera holder up/down
            _pitch -= mouseY;
            _pitch = Mathf.Clamp(_pitch, -85f, 85f);
            _cameraHolder.localRotation = Quaternion.Euler(_pitch, 0, 0);
        }
    }

    // FixedUpdateNetwork is for the networked physics simulation
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            // align player rotation with visuals rotation
            transform.rotation = _visualsRoot.rotation;

            // movement
            Vector3 moveDirection = transform.rotation * data.direction;
            _ncc.Move(moveDirection * Runner.DeltaTime * 10f);

            // jumping
            bool isGrounded = Mathf.Abs(_ncc.Velocity.y) < 0.1f;
            if (isGrounded && data.IsJumpPressed)
            {
                _ncc.Jump();
            }
        }
    }
}