using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    [SerializeField, Tooltip("The EXACT name of the Main Menu scene.")]
    private string mainMenuSceneName = "MainMenu";

    // Called when the Player clicks on the Quit button.
    public void OnQuitButtonClicked()
    {
        // Close the game.
        Application.Quit();
    }

    // Called when the Player clicks on the Main Menu button.
    public void OnMainMenuButtonClicked()
    {
        // Open up the main menu.
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
