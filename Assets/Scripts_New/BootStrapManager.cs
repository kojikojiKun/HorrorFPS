using UnityEngine;
using UnityEngine.SceneManagement;
public class BootStrapManager : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] initializers;

    private void Awake()
    {
        foreach (var obj in initializers)
        {

            if (obj == null)
                continue;

            if (obj is not IInitializeable initializeable)
                continue;

            //初期化が必要なオブジェクトを初期化.
            initializeable.Instantiate();
        }

        SceneManager.LoadScene("Title");
    }
}
