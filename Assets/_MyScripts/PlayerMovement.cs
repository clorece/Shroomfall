using UnityEngine;
using Fusion;

public class PlayerMovement : NetworkBehaviour
{
    private NetworkCharacterController _ncc;

    [Header("Camera Control")]
    [SerializeField] private Transform _cameraHolder;
    [SerializeField] private float _mouseSensitivity = 2.0f;

    private float _pitch = 0.0f;

    public override void Spawned()
    {
        _ncc = GetComponent<NetworkCharacterController>();

        if (Object.HasInputAuthority)
        {
            // Lock the cursor to the center of the screen and hide it
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            // only rotate the character for the local player.
            // NetworkTransform will sync the final rotation to other players.
            if (Object.HasInputAuthority)
            {
                _ncc.Move(data.direction * Runner.DeltaTime * 10f);

                if (data.isJumpPressed)
                {
                    _ncc.Jump();
                }

                // Get mouse input
                float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
                float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

                // Rotate the entire player body
                transform.Rotate(0, mouseX, 0);

                // Rotate the camera holder 
                _pitch -= mouseY;
                // Clamp prevent camera from flipping upside down
                _pitch = Mathf.Clamp(_pitch, -85f, 85f);
                _cameraHolder.localRotation = Quaternion.Euler(_pitch, 0, 0);
            }

            Vector3 moveDirection = transform.rotation * data.direction;
            _ncc.Move(moveDirection * Runner.DeltaTime * 10f);
        }
    }
}