using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class GameSaveData
{
    public bool finishedOpeningDialogue;
    public bool solvedPuzzleArea2;
    public bool unlockedMetro;
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameSaveData data = new GameSaveData();

    private string savePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            savePath = Application.persistentDataPath + "/gamedata.json";
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ===================== GETTER / SETTER =========================

    public void SetOpeningDialogueFinished(bool finished)
    {
        data.finishedOpeningDialogue = finished;
        SaveData();
    }

    public void SetPuzzleArea2Solved()
    {
        data.solvedPuzzleArea2 = true;
        SaveData();
    }

    // ===================== SAVE / LOAD =========================

    public void SaveData()
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game data saved.");
    }

    public void LoadData()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("No save file found, starting fresh.");
            return;
        }

        string json = File.ReadAllText(savePath);
        data = JsonUtility.FromJson<GameSaveData>(json);
        Debug.Log("Game data loaded.");
    }

    public void ResetGame()
    {
        data = new GameSaveData();
        SaveData();
        InventorySystem.Instance.ClearInventory();
        Debug.Log("Game reset.");
    }
}
