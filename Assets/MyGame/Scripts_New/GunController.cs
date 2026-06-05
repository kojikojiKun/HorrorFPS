using UnityEngine;
using System.Collections.Generic;

public class GunController : MonoBehaviour
{
    private InputReader inputReader;
    private GunMovement gunMovement;
    private Dictionary<int, GameObject> gunsInstances = new();
    private EquipType currentEquipType;
    private GameObject[] guns;

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
        foreach (var obj in gunsData)
        {
            //データの重複をチェック.
            if (gunsInstances.ContainsKey(obj.ID))
            {
                Debug.LogError($"重複キー: {obj.name}");
                continue;
            }

            //オブジェクト生成.
            GameObject gun = Instantiate(
                obj.GunPrefab,
                spawnGunPos.position,
                Quaternion.identity,
                gunsPivot
                );

            //Dictionary(Key:GunData.ID,Value:GameObject)に保存.
            gunsInstances.Add(obj.ID, gun);
        }

        //GunMovementスクリプトにシーン上の銃インスタンスを登録したDictionaryを渡す.
        gunMovement.GetGunInstancesDictionary(gunsInstances);

        //銃を装備.
        gunMovement.EquipGun(0);
    }

    private void HandleChangePrimary()
    {
        //プライマリ武器装備処理を実装.
        currentEquipType = EquipType.Primary;
    }

    private void HandleEquipSecondary()
    {
        //セカンダリー武器装備処理を実装.
        currentEquipType = EquipType.Secondary;
    }

    private void HandleFire(bool isPusshing )
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
