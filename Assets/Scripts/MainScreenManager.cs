using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScreenManager : MonoBehaviour
{
    public static MainScreenManager Instance { get; private set; }


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    public void ChangeToGameScene()
    {
        SceneManager.LoadScene("SampleScene"); // 실제 게임 씬 이름
    }

    public void ChangeToEndingScene()
    {
        SceneManager.LoadScene("EndingScene");
    }

    public void ChangeToMainScene()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void EndGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 종료
#endif
    }
}
