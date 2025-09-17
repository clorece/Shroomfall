
using UnityEngine;
using TMPro;

public class LobbyUIJoinBridge : MonoBehaviour {
  public NetworkRunnerHandler handler;
  public TMP_InputField codeInput;

  public void OnJoinClick() {
    var code = codeInput.text.Trim().ToUpperInvariant();
    handler.JoinGame(code);
  }
}
