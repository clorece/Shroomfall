using UnityEngine;
using TMPro;
public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _sessionNameInput;
    [SerializeField] private NetworkManager _networkManagerPrefab;

    public void CreateLobby()
    {
        string sessionName = _sessionNameInput.text;
        if (string.IsNullOrEmpty(_sessionNameInput.text))
        {
            Debug.LogError("Session name cannot be empty!");
            return;
        }
        
        NetworkManager manager = Instantiate(_networkManagerPrefab);
        // Start the game
        manager.StartGame(Fusion.GameMode.Host, sessionName);
    }

    public void OnJoinClicked()
    {
        string sessionName = _sessionNameInput.text;
        if (string.IsNullOrEmpty(_sessionNameInput.text))
        {
            Debug.LogError("Session name cannot be empty!");
            return;
        }
        NetworkManager manager = Instantiate(_networkManagerPrefab);
        // Join the game
        manager.StartGame(Fusion.GameMode.Client, sessionName);
    }


}
