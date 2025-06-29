using UnityEngine;
using UnityEngine.SceneManagement;  
public static class Data
{
    public static int currentLevel = 0;
    public static float timer = 0f;

    public static void Save()
    {
        PlayerPrefs.SetInt("currentLevel", currentLevel);
        PlayerPrefs.SetFloat("timer", timer);
        PlayerPrefs.Save();
    }

    public static void Load()
    {
        currentLevel = PlayerPrefs.GetInt("currentLevel", 0);
        timer = PlayerPrefs.GetFloat("timer", 0f);
    }
}
