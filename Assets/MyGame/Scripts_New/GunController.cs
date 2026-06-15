using UnityEngine;
using System.Collections.Generic;

public class GunController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform spawnGunPos;
    [SerializeField] private Transform gunsPivot;
    [SerializeField] private GunData[] gunsData;

    private InputReader inputReader;
    private GunInventory inventory;
    private GunMovement movement;
    private GunShooter shooter;

    private bool isPushing;

    private void Awake()
    {
        //nullチェック.
        if (InputManager.Instance == null)
            Debug.LogError("GunController inputManager.Instance is null");
        if (cam == null)
            Debug.LogError("Cam is null");
        if (spawnGunPos == null)
            Debug.LogError("SpawnGunPos is null");
        if (gunsPivot == null)
            Debug.LogError("GunsPivot is null");
        if (gunsData.Length == 0 || gunsData == null)
            Debug.LogError("GunsData is null");

        //オブジェクト取得.
        inputReader = InputManager.Instance.InputReader;

        //オブジェクト生成.
        inventory = new GunInventory();
        movement = new GunMovement(cam, gunsPivot);
        shooter = new GunShooter();

        //InputReaderのイベントに対応するメソッドを登録.
        inputReader.OnChangePrimary += HandleChangePrimary;
        inputReader.OnEquipSecondary += HandleEquipSecondary;
        inputReader.OnFire += SetFireInput;
        inputReader.OnAiming += HandleAiming;
    }

    private void OnDestroy()
    {
        //イベントの登録を解除して、メモリリークを防止.
        inputReader.OnChangePrimary -= HandleChangePrimary;
        inputReader.OnEquipSecondary -= HandleEquipSecondary;
        inputReader.OnFire -= SetFireInput;
        inputReader.OnAiming -= HandleAiming;
    }

    private void Start()
    {
        inventory.InstantiateGuns(gunsData,spawnGunPos,gunsPivot);
    }

    private void HandleChangePrimary()
    {
        //プライマリー武器への変更を要求.
        ReqestChangeGun(EquipType.Primary);
    }

    private void HandleEquipSecondary()
    {
        //セカンダリー武器への変更を要求.
        ReqestChangeGun(EquipType.Secondary);
    }

    private void ReqestChangeGun(EquipType type)
    {
        //装備中の銃を切り替え.
        inventory.ChangeGun(type);

        //装備した銃のデータをGunShooterクラスに渡す.
        shooter.SetCurrentEquipGunData(inventory.CurrentGunData);
    }

    private void SetFireInput(bool isPushingButton)
    {
        isPushing = isPushingButton;
    }

    private void HandleReLoad()
    {
        //銃のマガジンリロード処理を実装.
    }

    private void HandleAiming(bool isPusshing)
    {
        //銃のエイム処理を実装.
    }

    private void Update()
    {
        movement.FollowCamera();
        shooter.UpDateFire(isPushing);
    }
}
