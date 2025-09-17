using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyNavigation : MonoBehaviour {
  [SerializeField] string customizationScene = "PlayerCustomization"; //build index 2

  public void GoToCustomization() {
    SceneManager.LoadScene(customizationScene, LoadSceneMode.Single);
  }
}
