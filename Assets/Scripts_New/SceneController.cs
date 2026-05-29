using UnityEngine;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        //インスタンス化し、シーンをまたいでも破壊しない.
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}
