using UnityEngine;

public enum ItemType
{
    notKey,
    key
}

//このスクリプトをアイテムにアタッチ
//アイテムのタグに[lifle],[shotgun],[PistolBullet][MachineGunBullet],[ShotGunBullet],[HealItem]を設定

public class Item : MonoBehaviour
{
    //鍵かそれ以外のアイテムかを設定
    public ItemType itemType;

    public GameObject ui;
    private GameObject uiInstance;

    // UIを置くWorld Space CanvasのTransform（親）
    public Transform uiParent;
    public float offset;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // UIを生成して表示
            if (uiInstance == null)
            {
                uiInstance = Instantiate(ui, uiParent);
            }
            else
            {
                Debug.LogWarning("not found image");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // UIを消す
            if (uiInstance != null)
            {
                Destroy(uiInstance);
                uiInstance = null;
            }
        }
    }

    private bool isUsed = false;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            if (itemType == ItemType.key && isUsed==false)
            {
                isUsed = true; // 1回だけ実行させるフラグ
                Escape.instanse.needKey--; //取得したアイテムが鍵なら必要な鍵の数を-1
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                PlayerMovementScript pms = player.GetComponent<PlayerMovementScript>();
                pms.pickUpSound.Play();
            }

            Destroy(uiInstance);
            uiInstance = null;
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (uiInstance != null)
        {
            // アイテムの少し上のワールド座標を計算
            Vector3 uiWorldPos = transform.position + Vector3.up * offset;
            uiInstance.transform.position = uiWorldPos;

            //UIの向きをカメラに向ける
            uiInstance.transform.LookAt(Camera.main.transform);
            uiInstance.transform.Rotate(0, 180f, 0);  // 反転調整
        }
    }
}
