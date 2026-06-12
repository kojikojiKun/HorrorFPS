using UnityEngine;

[System.Serializable]
public class PlayerStatus
{
    public float MoveSpeed;
}

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerStatus status;
    [SerializeField] private Camera cam;

    private CharacterController characterController;
    private InputReader inputReader;
    private PlayerMovement movement;    
    private Vector2 inputDir;
    
    private void Awake()
    {
        if (InputManager.Instance == null)
        {
            Debug.LogError("PlayerController inputReader.Instance is Null");
            return;
        }

        //InputManagerインスタンス取得.
        inputReader = InputManager.Instance.InputReader;

        //コンポーネント取得.
        characterController = GetComponent<CharacterController>();

        //オブジェクト生成.
        movement = new PlayerMovement(characterController, cam.transform, status);

        //InputReaderのイベントに対応するメソッドを登録.
        inputReader.OnMove += HandleMove;
        inputReader.OnLook += HandleLook;
        inputReader.OnRun += HandleRun;
        inputReader.OnOpenMenu += HandleOpenMenu;
    }

    private void OnDestroy()
    {
        //イベントの登録を解除して、メモリリークを防止.
        inputReader.OnMove -= HandleMove;
        inputReader.OnLook -= HandleLook;
        inputReader.OnRun -= HandleRun;
        inputReader.OnOpenMenu -= HandleOpenMenu;
    }

    private void HandleMove(Vector2 input)
    {
        //移動処理をここに実装.
        inputDir = input;
    }

    private void HandleLook(Vector2 lookInput)
    {
        //視点移動処理をここに実装.
    }

    private void HandleRun(bool isPusshing)
    {
        //走る処理をここに実装.
    }

    private void HandleOpenMenu()
    {
        //メニューを開く処理をここに実装.
    }

    private void Update()
    {
        movement.Move(inputDir);
    }
}
