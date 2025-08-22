using UnityEngine;
using Fusion;

public class PlayerMovement : NetworkBehaviour
{
    public static PlayerMovement Local { get; private set; }

    private NetworkCharacterController _ncc;

    [Header("Camera Control")]
    [SerializeField] private Transform _cameraHolder;
    [SerializeField] private float _mouseSensitivity = 2.0f;

    private float _pitch = 0.0f;
    private float _accumulatedYaw = 0.0f;

    public override void Spawned()
    {
        _ncc = GetComponent<NetworkCharacterController>();

        if (Object.HasInputAuthority)
        {
            Local = this;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public float GetAccumulatedYawAndReset()
    {
        float yaw = _accumulatedYaw;
        _accumulatedYaw = 0f; // Reset for the next tick
        return yaw;
    }

    // Update() is for smooth, local, visual-only effects.
    // Perfect for a first-person camera.
    void Update()
    {
        // We only run this code for the player we are controlling.
        if (Object.HasInputAuthority)
        {
            // Accumulate the horizontal mouse input every frame
            _accumulatedYaw += Input.GetAxis("Mouse X") * _mouseSensitivity;

            // Handle the vertical camera pitch locally every frame for smoothness
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;
            _pitch -= mouseY;
            _pitch = Mathf.Clamp(_pitch, -85f, 85f);
            _cameraHolder.localRotation = Quaternion.Euler(_pitch, 0, 0);
        }
    }

    // FixedUpdateNetwork() is for physics and simulation-changing logic.
    public override void FixedUpdateNetwork()
    {
        // GetInput gets the WASD/Jump data from our NetworkManager's OnInput.
        if (GetInput(out NetworkInputData data))
        {
            transform.Rotate(0, data.Yaw, 0);
            // --- MOVEMENT ---
            // The movement uses the rotation that was applied visually in Update().
            Vector3 moveDirection = transform.rotation * data.direction;
            _ncc.Move(moveDirection * Runner.DeltaTime * 10f);

            // --- JUMP ---
            // The jump logic uses the most reliable check we found.
            if (data.IsJumpPressed && _ncc.Grounded)
            {
                _ncc.Jump();
            }
        }
    }
}