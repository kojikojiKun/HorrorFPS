using UnityEditor;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class PlayModeEditor
{
    static string m_bootSceneName = "Bootstrap";// ←移動したいシーンの名前を記入する

    static PlayModeEditor()
    {
        EditorApplication.playModeStateChanged += ChangeBootScene;
    }

    static void ChangeBootScene(PlayModeStateChange state)
    {
        // 実行状態になったら
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            // 別シーンで起動していた場合切り替える
            Scene scene = SceneManager.GetActiveScene();
            if (!scene.Equals(m_bootSceneName))
            {
                SceneManager.LoadScene(m_bootSceneName);
            }
        }
    }
}

