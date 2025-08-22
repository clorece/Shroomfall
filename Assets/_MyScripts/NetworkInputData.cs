using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector3 direction;
    public NetworkBool IsJumpPressed;
    public float Yaw;
    public float Pitch;
}