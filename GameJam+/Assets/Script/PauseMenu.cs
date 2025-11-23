using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject PausePanel;
    public GameObject SettingsPanel;

    private bool isPaused = false;

    void Start()
    {
        // pastikan saat scene yang punya PauseMenu dibuka, game TIDAK pause
        Time.timeScale = 1f;
        isPaused = false;

        if (PausePanel != null) PausePanel.SetActive(false);
        if (SettingsPanel != null) SettingsPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        if (PausePanel != null) PausePanel.SetActive(true);
        if (SettingsPanel != null) SettingsPanel.SetActive(false);

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        if (PausePanel != null) PausePanel.SetActive(false);
        if (SettingsPanel != null) SettingsPanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    public void OpenSettings()
    {
        if (SettingsPanel != null) SettingsPanel.SetActive(true);
        if (PausePanel != null) PausePanel.SetActive(false);
    }

    public void CloseSettings()
    {
        if (SettingsPanel != null) SettingsPanel.SetActive(false);
        if (PausePanel != null) PausePanel.SetActive(true);
    }

    public void QuitToMainMenu()
    {
        // 🔹 WAJIB: balikin timeScale dulu
        Time.timeScale = 1f;
        isPaused = false;

        // pastikan tidak ada panel yang ikut kebawa
        if (PausePanel != null) PausePanel.SetActive(false);
        if (SettingsPanel != null) SettingsPanel.SetActive(false);

        SceneManager.LoadScene("MainMenu");  // ganti dengan nama scene main menu kamu
    }
}
