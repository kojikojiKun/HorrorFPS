using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public GameObject panelInstruction; // 操作説明パネル
    public GameObject panelVolume;      // 音量調整パネル

   

    // ボタンから呼ぶ
    public void ShowVolumePanel()
    {
        panelInstruction.SetActive(false);
        panelVolume.SetActive(true);
    }

    public void ShowInstructionPanel()
    {
        panelInstruction.SetActive(true);
        panelVolume.SetActive(false);
    }
}
