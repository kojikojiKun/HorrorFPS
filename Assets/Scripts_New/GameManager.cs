using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IInitializeable
{
    private bool isInitialized = false;

    public static GameManager Instance;
    public bool IsInitialized => isInitialized;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public async Task InitializeAsync() { await Task.Delay(0); }

    public void Instantiate()
    {
        if (isInitialized)
            return;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        //インスタンス化し、シーンをまたいでも破壊しない.
        Instance = this;
        DontDestroyOnLoad(gameObject);

        isInitialized = true;
    }

    //シーンごとに必要な処理を実行.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Title":
                break;
            case "Play":
                break;
        }
    }
}
