using UnityEngine;
using UnityEngine.SceneManagement;
public class BootStrapManager : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] initializers;

    private async void Start()
    {
        foreach (var obj in initializers)
        {

            if (obj == null)
                continue;

            if (obj is not IInitializeable initializeable)
                continue;

            //初期化が必要なオブジェクトが初期化できるまで待機.
            await initializeable.InitializeAsync();
        }

        SceneManager.LoadScene("Title");
    }
}
