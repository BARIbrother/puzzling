using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScreenManager : MonoBehaviour
{
    public void ChangeToGameScene()
    {
        SceneManager.LoadScene("SampleScene"); // 실제 게임 씬 이름
    }

    public void EndGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 종료
#endif
    }
}
