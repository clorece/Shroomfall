using UnityEngine;
using Fusion;

public class PlayerMovement : NetworkBehaviour
{
    private NetworkCharacterController _ncc;

    [Header("Camera Control")]
    [SerializeField] private Transform _cameraHolder;
    [SerializeField] private float _mouseSensitivity = 2.0f;

    [Networked] private float NetworkedPitch { get; set; }

    public override void Spawned()
    {
        _ncc = GetComponent<NetworkCharacterController>();

        if (Object.HasInputAuthority)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public override void Render()
    {
        // The camera still updates in Render for the smoothest possible result from our NetworkedPitch
        _cameraHolder.localRotation = Quaternion.Euler(NetworkedPitch, 0, 0);
    }

    // All logic now happens in the network simulation tick
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            // HORIZONTAL ROTATION (YAW)
            transform.Rotate(0, data.Yaw * _mouseSensitivity, 0);

            // VERTICAL ROTATION (PITCH)
            float pitch = NetworkedPitch;
            pitch -= data.Pitch * _mouseSensitivity;
            NetworkedPitch = Mathf.Clamp(pitch, -85f, 85f);

            // MOVEMENT
            Vector3 moveDirection = transform.rotation * data.direction;
            _ncc.Move(moveDirection * Runner.DeltaTime * 10f);

            // JUMP
            bool isGrounded = Mathf.Abs(_ncc.Velocity.y) < 0.1f;
            if (isGrounded && data.IsJumpPressed)
            {
                _ncc.Jump();
            }
        }
    }
}