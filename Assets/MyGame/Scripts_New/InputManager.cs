using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(ControllerReader))]
[RequireComponent(typeof(InputReader))]
public class InputManager : MonoBehaviour, IDontDestroy
{
    public static InputManager Instance { get; private set; }
    private ControllerReader controllerReader;
    private InputReader inputReader;
    private static Gamepad pad;
    private bool isInitialized = false;

    public InputReader InputReader => inputReader;
    public static Gamepad CurrentPad => pad;
    public bool IsInitialized => isInitialized;

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

        controllerReader = GetComponent<ControllerReader>();
        inputReader = GetComponent<InputReader>();

        isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (controllerReader != null)
            //使用中のコントローラーを登録.
            pad = controllerReader.CurrentGamePad();
    }
}
