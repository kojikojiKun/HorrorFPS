using UnityEngine;
using TMPro;

//このスクリプトを空のオブジェクトにアタッチ
public class Escape : MonoBehaviour
{
    [Tooltip("脱出に必要な鍵の数")]
    public int needKey;

    public TextMeshProUGUI textObj; //インスペクタで設定

    public bool canEscape = false;　//脱出可能？
    public static Escape instanse;

    private void Awake()
    {
        if (instanse == null)
        {
            instanse = this;
        }
    }

    private void Start()
    {
        textObj.enabled = false;
    }

    //テキストに現在の必要な鍵の数を表示
    public void Text()
    {
        textObj.text = "NeedKeys..." + needKey;
    }

    private void Update()
    {
        EscapePreparation();
    }

    //必要な鍵数が0になると脱出可能
    public void EscapePreparation()
    {
        if (needKey == 0)
        {
            canEscape = true;
        }
    }
}
