using UnityEngine;

public class OpenPanel : MonoBehaviour
{
    public GameObject panel;  // 表示するPanel

    public void Open()
    {
        panel.SetActive(true);  // パネルを表示
    }

    public void Close()
    {
        panel.SetActive(false);
    }

}
