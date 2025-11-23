using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string gameSceneName = "Area 1";
    public GameObject settingsPanel;
    public GameObject creditPanel;

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void OpenCredits()
    {
        creditPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void CloseCredits()
    {
        creditPanel.SetActive(false);
    }

}
