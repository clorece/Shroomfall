using Fusion;
using UnityEngine;

// wow this whole thing is dumb
// need this to fix problem with input not registering in PlayerMovement.cs
public struct NetworkInputData : INetworkInput
{
    public Vector3 direction;
}