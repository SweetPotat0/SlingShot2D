using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreManager
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="LevelNumber"></param>
    /// <returns>The score at the current level or -1 if no high score exists</returns>
    public static float GetHighScore(int LevelNumber)
    {
        if (PlayerPrefs.HasKey($"HighScore{LevelNumber}"))
        {
            return PlayerPrefs.GetFloat($"HighScore{LevelNumber}");
        }
        else
        {
            return -1;
        }
    }

    public static void StoreHighScore(int LevelNumber, float HighScore)
    {
        if (GetHighScore(LevelNumber) < HighScore)
        {
            PlayerPrefs.SetFloat($"HighScore{LevelNumber}", HighScore);
        }
    }
}
