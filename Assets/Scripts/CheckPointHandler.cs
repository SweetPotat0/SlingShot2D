using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointHandler
{
    public struct CheckPoint
    {
        public int Index;
        public float Time;
    }

    public static CheckPoint GetCheckPoint(int levelNumber)
    {
        if (PlayerPrefs.HasKey($"CheckPointIndex{levelNumber}"))
        {
            int index = PlayerPrefs.GetInt($"CheckPointIndex{levelNumber}");
            float time = PlayerPrefs.GetFloat($"CheckPointTime{levelNumber}");
            return new CheckPoint { Index = index, Time = time };
        }
        else
        {
            Debug.Log($"No check point for level: {levelNumber}");
            return new CheckPoint { Index = -1, Time = 0 };
        }
    }

    public static void SaveCheckPoint(int levelNum, int checkpointIndex, float time)
    {
        Debug.Log($"Saving checkpoint: index:{checkpointIndex}, time:{time}, level: {levelNum}");
        CheckPoint checkpoint = GetCheckPoint(levelNum);
        if (checkpoint.Index < checkpointIndex)
        {
            PlayerPrefs.SetInt($"CheckPointIndex{levelNum}", checkpointIndex);
            PlayerPrefs.SetFloat($"CheckPointTime{levelNum}", time);
        }
    }
}
