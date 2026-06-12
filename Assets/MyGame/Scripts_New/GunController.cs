using UnityEngine;
using System.Collections.Generic;

public class GunController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform spawnGunPos;
    [SerializeField] private Transform gunsPivot;
    [SerializeField] private GunData[] gunsData;

    private InputReader inputReader;
    private GunMovement gunMovement;
    private Dictionary<GunData, GameObject> gunsInstances = new();
    private GunData[] primariesData = new GunData[2];
    private GunData secondaryData;

    private void Awake()
    {
        //nullチェック.
        if (InputManager.Instance == null)
        {
            Debug.LogError("GunController inputManager.Instance is null");
            return;
        }

        //nullチェック.
        if (cam == null || spawnGunPos == null || gunsPivot == null || gunsData.Length == 0)
        {
            Debug.LogError(
                $"Camara is null? {cam == null}," +
                $"SpawnGunPos is null? {spawnGunPos == null}," +
                $"GunsPivot is null? {gunsPivot == null}," +
                $"gunsData is null? {gunsData.Length == 0}"
                );
            return;
        }

        //InputManagerインスタンス取得.
        inputReader = InputManager.Instance.InputReader;

        //オブジェクト生成.
        gunMovement = new GunMovement(cam, gunsPivot);

        //InputReaderのイベントに対応するメソッドを登録.
        inputReader.OnChangePrimary += HandleChangePrimary;
        inputReader.OnEquipSecondary += HandleEquipSecondary;
        inputReader.OnFire += HandleFire;
        inputReader.OnAiming += HandleAiming;
    }

    private void OnDestroy()
    {
        //イベントの登録を解除して、メモリリークを防止.
        inputReader.OnChangePrimary -= HandleChangePrimary;
        inputReader.OnEquipSecondary -= HandleEquipSecondary;
        inputReader.OnFire -= HandleFire;
        inputReader.OnAiming -= HandleAiming;
    }

    private void Start()
    {
        int index = 0;

        foreach (var data in gunsData)
        {
            //データの重複をチェック.
            if (gunsInstances.ContainsKey(data))
            {
                Debug.LogError($"重複キー: {data.name}");
                continue;
            }

            //オブジェクト生成.
            GameObject gun = Instantiate(
                data.GunPrefab,
                spawnGunPos.position,
                Quaternion.identity,
                gunsPivot
                );

            //Dictionary(Key:GunsData,Value:GameObject)に追加.
            gunsInstances.Add(data, gun);

            //武器をプライマリー武器とセカンダリー武器に分類.
            if (data.EquipType == EquipType.Secondary)
            {
                secondaryData = data;
            }
            else
            {
                primariesData[index] = data;
                index++;
            }
        }

        //nullチェック.
        if (primariesData.Length == 0 || secondaryData == null)
        {
            Debug.LogError(
                $"PrimariesData_Length:{primariesData.Length}," +
                $"SecondaryData is null?:{secondaryData == null}"
                );
            return;
        }

        //GunMovementスクリプトに必要な要素を渡す.
        gunMovement.GetGunsData(gunsInstances, primariesData, secondaryData);

        //セカンダリー武器を装備.
        if (secondaryData != null)
            gunMovement.EquipGun(secondaryData);
            
    }

    private void HandleChangePrimary()
    {
        //プライマリー武器への変更を要求.
        gunMovement.ChangeGun(EquipType.Primary);
    }

    private void HandleEquipSecondary()
    {
        //セカンダリー武器への変更を要求.
        gunMovement.ChangeGun(EquipType.Secondary);
    }

    private void HandleFire(bool isPusshing)
    {
        //射撃処理を実装.

        //入力がある間だけ実行.
        if (!isPusshing)
            return;
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
        gunMovement.FollowCamera();
    }
}
