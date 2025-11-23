using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string gameSceneName = "Area 1";
    public GameObject MainMenuPanel;
    public GameObject SettingPanel;
    public GameObject CreditPanel;


    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenSettings()
    {
        SettingPanel.SetActive(true);
        MainMenuPanel.SetActive(false);
    }

    public void OpenCredits()
    {
        CreditPanel.SetActive(true);
        MainMenuPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }

    public void CloseSettings()
    {
        SettingPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        CreditPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

}
