using UnityEngine;

public class GunController : MonoBehaviour
{
    private InputReader inputReader;
    private int currentGunIndex;
    private int lastGunIndex;

    [SerializeField] private Transform spawnGunPos;
    [SerializeField] private GunData[] mainGuns;
    [SerializeField] private GunData subGun;

    private void Awake()
    {
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
        //銃オブジェクト生成.
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
}
