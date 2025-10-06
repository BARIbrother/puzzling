using UnityEngine;

public class SaverLoader : MonoBehaviour
{
    public static void SaveProgress(int StageIndex, int PuzzleIndex, int DNum)
    {
        PlayerPrefs.SetInt("StageIndex", StageIndex);
        PlayerPrefs.SetInt("PuzzleIndex", PuzzleIndex);
        PlayerPrefs.SetInt("DNum", DNum);
        PlayerPrefs.Save();
    }

    public static void SaveSettings(float bgm, float logSensitivity)
    {
        PlayerPrefs.SetFloat("bgmVolume", bgm);
        PlayerPrefs.SetFloat("logSensitivity", logSensitivity);
        PlayerPrefs.Save();
    }

    // 불러오기
    public static int LoadStageIndex() => PlayerPrefs.GetInt("StageIndex", -1);
    public static int LoadPuzzleIndex() => PlayerPrefs.GetInt("PuzzleIndex", 0);
    public static int LoadDNum() => PlayerPrefs.GetInt("DNum", 0);
    public static float LoadBgm() => PlayerPrefs.GetFloat("bgmVolume", 0f);
    public static float LoadSensitivity() => PlayerPrefs.GetFloat("logSensitivity", 1f);
}
