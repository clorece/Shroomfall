using UnityEngine;
using Fusion;

public class PlayerMovement : NetworkBehaviour
{
    private NetworkCharacterController _ncc;

    // Spawned() is a special Fusion function that is called after a network object is created.
    // It's a great place to get references to other components.
    public override void Spawned()
    {
        _ncc = GetComponent<NetworkCharacterController>();
    }

    // All movement code
    public override void FixedUpdateNetwork()
    {
        // GetInput call works now, NetworkManager providing input
        if (GetInput(out NetworkInputData data))
        {
            _ncc.Move(data.direction * Runner.DeltaTime * 10f);
        }
    }
}