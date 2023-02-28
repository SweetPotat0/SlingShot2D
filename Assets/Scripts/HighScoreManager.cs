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

    public static bool StoreHighScore(int LevelNumber, float HighScore)
    {
        float storedHighScore = GetHighScore(LevelNumber);
        Debug.Log($"Stored is: {storedHighScore}, and curr is: {HighScore}");
        if (storedHighScore == -1 || storedHighScore > HighScore)
        {
            PlayerPrefs.SetFloat($"HighScore{LevelNumber}", HighScore);
            if (storedHighScore != -1)
            {
                return true;
            }
        }
        return false;
    }
}
