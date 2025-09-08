using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplyCustomizationButton : MonoBehaviour {
    [SerializeField] ShroomCustomizerMPB customizer;   // drag your preview shroom here
    [SerializeField] string gameSceneName = "GameScene"; // change if your scene name is different

    public void ApplyAndGo() {
        // Save chosen look
        CharacterSelection.Data = customizer.GetData();

        // Go to GameScene
        SceneManager.LoadScene(gameSceneName);
    }
}
