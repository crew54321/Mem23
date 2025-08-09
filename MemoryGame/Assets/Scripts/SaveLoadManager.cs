using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int score;
    public float timeRemaining;
    public List<int> matchedCardIDs = new List<int>(); // stores the CardID of matched cards
}

public static class SaveLoadManager
{
    private const string SaveKey = "MemoryGameSave";

    public static void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
        Debug.Log("Game Saved: " + json);
    }

    public static SaveData LoadGame()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string json = PlayerPrefs.GetString(SaveKey);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Game Loaded: " + json);
            return data;
        }
        return null; // no save found
    }

    public static void ClearSave()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        Debug.Log("Save Cleared");
    }
}
