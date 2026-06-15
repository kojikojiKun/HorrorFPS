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

            if (obj is not IDontDestroy dontDestroy)
                continue;

            //シーンをまたいで存在させるオブジェクトを初期化.
            dontDestroy.Instantiate();
        }

        SceneManager.LoadScene("Title");
    }
}
