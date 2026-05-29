using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private InputReader inputReader;

    private void Awake()
    {
        //InputReaderのイベントに対応するメソッドを登録.
        inputReader.OnMove += HandleMove;
        inputReader.OnLook += HandleLook;
        inputReader.OnChangeGun += HandleChangeGun;
        inputReader.OnRun += HandleRun;
        inputReader.OnFire += HandleFire;
        inputReader.OnAiming += HandleAiming;
        inputReader.OnOpenMenu += HandleOpenMenu;
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

    private void HandleMove(Vector2 direction)
    {
        //移動処理をここに実装.
    }

    private void HandleLook(Vector2 lookDirection)
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
