using UnityEngine;

public enum DoorType
{
    nonEscape,
    Escape
}

public class SlidingDoor : MonoBehaviour
{
    public DoorType type;
    public Transform leftDoor;  // 左ドア
    public Transform rightDoor; // 右ドア

    public float slideDistance = 2f;    // 開く距離
    public float slideSpeed = 2f;       // 開閉速度
    public float openDuration = 3f;

    private Vector3 leftClosedPos;
    private Vector3 rightClosedPos;
    private Vector3 leftOpenPos;
    private Vector3 rightOpenPos;

    private bool isOpening = false;
    private float openTimer = 0f;

    private bool isPlayerNear = false;

    void Start()
    {
        // 初期位置を記録
        leftClosedPos = leftDoor.localPosition;
        rightClosedPos = rightDoor.localPosition;

        leftOpenPos = leftClosedPos + new Vector3(0f, 0f,-slideDistance);   // Z軸プラス方向に左ドア移動
        rightOpenPos = rightClosedPos + new Vector3(0f, 0f, slideDistance); // Z軸マイナス方向に右ドア移動
    }

    void Update()
    {
        if (type == DoorType.Escape && Escape.instanse.canEscape == true)
        {
            Open();
        }

        if (type == DoorType.nonEscape)
        {
            Open();
        }
    }

    //ドアを開く
    void Open()
    {
        // 一回押したら開くトリガー
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            isOpening = true;
            openTimer = openDuration;  // タイマーリセット
        }

        if (isOpening)
        {
            // 開く
            leftDoor.localPosition = Vector3.MoveTowards(leftDoor.localPosition, leftOpenPos, slideSpeed * Time.deltaTime);
            rightDoor.localPosition = Vector3.MoveTowards(rightDoor.localPosition, rightOpenPos, slideSpeed * Time.deltaTime);

            // タイマー減らす
            openTimer -= Time.deltaTime;
            if (openTimer <= 0f)
            {
                isOpening = false;  // 閉じるモードに切り替え
            }
        }
        else
        {
            leftDoor.localPosition = Vector3.MoveTowards(leftDoor.localPosition, leftClosedPos, slideSpeed * Time.deltaTime);
            rightDoor.localPosition = Vector3.MoveTowards(rightDoor.localPosition, rightClosedPos, slideSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true; 
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;

        }
    }
}
