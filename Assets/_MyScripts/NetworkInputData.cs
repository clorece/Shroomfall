using Fusion;
using UnityEngine;

// Collects all of our input for the current tick
public struct NetworkInputData : INetworkInput
{
    public Vector3 direction;
    public NetworkBool IsJumpPressed;
    // public float Yaw;
    // public float Pitch;
}