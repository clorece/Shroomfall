//STILL needs optimization (Vector2 is 2 floats = 8 bytes, instead could send 2 bytes)
using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector2 movementInput;
    public NetworkBool isJumpPressed;
}