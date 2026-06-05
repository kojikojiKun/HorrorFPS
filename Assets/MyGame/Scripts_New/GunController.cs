using UnityEngine;
using System.Collections.Generic;

public class GunController : MonoBehaviour
{
    private InputReader inputReader;
    private GunMovement gunMovement;
    private Dictionary<GunType, GameObject> gunPrefabs = new();
    private Dictionary<GunType, GameObject> gunInstances = new();

    [SerializeField] private Camera cam;
    [SerializeField] private Transform spawnGunPos;
    [SerializeField] private Transform gunsPivot;
    [SerializeField] private GunData[] guns;

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
        inputReader.OnChangeMainGun += HandleChangeMainGun;
        inputReader.OnChangeSubGun += HandleChangeSubGun;
        inputReader.OnFire += HandleFire;
        inputReader.OnAiming += HandleAiming;
    }

    private void OnDestroy()
    {
        //イベントの登録を解除して、メモリリークを防止.
        inputReader.OnChangeMainGun -= HandleChangeMainGun;
        inputReader.OnChangeSubGun -= HandleChangeSubGun;
        inputReader.OnFire -= HandleFire;
        inputReader.OnAiming -= HandleAiming;
    }

    private void Start()
    {
        foreach (var obj in guns)
        {
            Debug.Log($"Name:{obj.name}  GunType:{obj.GunType}");

            //Dictionaryにプレファブを保存.
            gunPrefabs.Add(obj.GunType, obj.GunPrefab);

            //オブジェクト生成.
            GameObject gun = Instantiate(
                obj.GunPrefab,
                spawnGunPos.position,
                Quaternion.identity,
                gunsPivot
                );

            //オブジェクト非表示.
            gun.SetActive(false);

            //Dictionaryにインスタンスを保存.
            gunInstances.Add(obj.GunType, gun);
        }

        //GunMovementスクリプトにシーン上の銃インスタンスを登録したDictionaryを渡す.
        gunMovement.GetGunInstancesDictionary(gunInstances);

        //銃を装備.
        gunMovement.Equip(GunType.Pistol);
    }

    private void HandleChangeMainGun()
    {
        //メイン武器変更処理を実装.
    }

    private void HandleChangeSubGun()
    {
        //サブ武器変更処理を実装.
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
