using UnityEngine;

public class PlayerStatus
{
    public float MoveSpeed;
}

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private InputReader inputReader;
    private PlayerMovement movement;
    private PlayerStatus status;

    [SerializeField] private float moveSpeed;
    [SerializeField] private Camera cam;

    private void Awake()
    {
        if (InputManager.Instance != null)
            inputReader = InputManager.Instance.InputReader;

        //InputReaderのイベントに対応するメソッドを登録.
        inputReader.OnMove += HandleMove;
        inputReader.OnLook += HandleLook;
        inputReader.OnChangeGun += HandleChangeGun;
        inputReader.OnRun += HandleRun;
        inputReader.OnFire += HandleFire;
        inputReader.OnAiming += HandleAiming;
        inputReader.OnOpenMenu += HandleOpenMenu;

        //ステータス代入.
        status = new PlayerStatus
        {
            MoveSpeed = moveSpeed,
        };

        //コンポーネント取得.
        characterController = GetComponent<CharacterController>();

        //オブジェクト生成.
        movement = new PlayerMovement(characterController, cam.transform, status);
    }

    private void OnDestroy()
    {
        //イベントの登録を解除して、メモリリークを防止.
        inputReader.OnMove -= HandleMove;
        inputReader.OnLook -= HandleLook;
        inputReader.OnChangeGun -= HandleChangeGun;
        inputReader.OnRun -= HandleRun;
        inputReader.OnFire -= HandleFire;
        inputReader.OnAiming -= HandleAiming;
        inputReader.OnOpenMenu -= HandleOpenMenu;
    }

    private void HandleMove(Vector2 input)
    {
        //移動処理をここに実装.
        movement.Move(input);
    }

    private void HandleLook(Vector2 lookInput)
    {
        //視点移動処理をここに実装.
    }

    private void HandleChangeGun()
    {
        //銃の切り替え処理をここに実装.
    }

    private void HandleRun(bool isRunning)
    {
        //走る処理をここに実装.
    }
    private void HandleFire(bool isFiring)
    {
        //射撃処理をここに実装.
    }
    private void HandleAiming(bool isAiming)
    {
        //エイム処理をここに実装.
    }
    private void HandleOpenMenu()
    {
        //メニューを開く処理をここに実装.
    }
}
