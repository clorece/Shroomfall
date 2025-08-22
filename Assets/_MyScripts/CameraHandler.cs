using UnityEngine;
using Fusion;

public class CameraHandler : MonoBehaviour
{
    void Start()
    {
        // Find the NetworkObject component on the parent object.
        NetworkObject networkObject = GetComponentInParent<NetworkObject>();

        // If we have input authority over this player object, it's ours.
        // Otherwise, it's a remote player.
        if (networkObject != null && networkObject.HasInputAuthority)
        {
            // This is our local player, so we keep the camera and audio listener.
            gameObject.SetActive(true);
        }
        else
        {
            // This is a remote player, we don't need their camera.
            // Destroying it is the cleanest way to handle it.
            Destroy(gameObject);
        }
    }
}