using UnityEngine;
using System.Collections.Generic;

public class GunController : MonoBehaviour
{
    private InputReader inputReader;
    private GunMovement gunMovement;
    private Dictionary<GunData, GameObject> gunsInstances = new();
    private int primariesIndex = 0;
    private GunData[] primariesData = new GunData[2];
    private GunData secondaryData;
    private GunData currentEquipGunData;

    [SerializeField] private Camera cam;
    [SerializeField] private Transform spawnGunPos;
    [SerializeField] private Transform gunsPivot;
    [SerializeField] private GunData[] gunsData;

    private void Awake()
    {
        if (InputManager.Instance == null)
        {
            Debug.LogError("GunController inputManager.Instance is null");
            return;
        }

        //InputManagerインスタンス取得.
        inputReader = InputManager.Instance.InputReader;

        //オブジェクト生成.
        gunMovement = new GunMovement(cam, gunsPivot);

        //InputReaderのイベントに対応するメソッドを登録.
        inputReader.OnChangePrimary += HandleChangePrimary;
        inputReader.OnEquipSecondary += HandleEquipSecondary;
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

        //GunMovementスクリプトにシーン上の銃インスタンスを登録したDictionaryを渡す.
        gunMovement.GetGunInstancesDictionary(gunsInstances);

        //セカンダリー武器を装備.
        if (secondaryData != null)
            gunMovement.EquipGun(secondaryData);

        //装備中の銃のデータを登録.
        currentEquipGunData = secondaryData;
    }

    //プライマリ武器装備処理を実装.
    private void HandleChangePrimary()
    {
        GunData data = new GunData();

        //セカンダリー武器を装備中のとき.
        if (currentEquipGunData.EquipType == EquipType.Secondary)
        {
            //最後に装備していたプライマリー武器のデータを登録.
            data = primariesData[primariesIndex];
        }

         primariesIndex++;
            if (primariesIndex >= primariesData.Length)
                primariesIndex = 0;

        //武器を装備.
        gunMovement.EquipGun(data);

        //装備中の銃データを登録.
        currentEquipGunData = data;
    }

    //セカンダリー武器装備処理を実装.
    private void HandleEquipSecondary()
    {
        GunData data = new GunData();

        //すでにセカンダリー武器を装備中のとき.
        if (currentEquipGunData.EquipType == EquipType.Secondary)
        {
            //最後に装備していたプライマリー武器のデータを登録.
            data = primariesData[primariesIndex];
        }
        else
        {
            data = secondaryData;
        }

        //武器を装備.
        gunMovement.EquipGun(data);

        //装備中の銃データを登録.
        currentEquipGunData = data;
    }

    private void HandleFire(bool isPusshing)
    {
        //射撃処理を実装.
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
