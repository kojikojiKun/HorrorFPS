using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public static QuitGame instance;
    private void Awake()
    {
        instance = this;
        if (instance != null)
        {
            DontDestroyOnLoad(instance);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // エディタ上では停止、ビルド後は終了
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
