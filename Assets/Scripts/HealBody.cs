using UnityEngine;

public class HealBody : MonoBehaviour
{
    private GameObject textObject;
    private HPText hpText;
    private GameObject DamageController;
    private TakeDamagePlayer TDP;

    private void Start()
    {
        DamageController = GameObject.FindGameObjectWithTag("Player");
        TDP = DamageController.GetComponent<TakeDamagePlayer>();        
        textObject = GameObject.FindGameObjectWithTag("Text");
        hpText = textObject.GetComponent<HPText>();
    }

    // 回復処理
    public void HealPart(int partId)
    {
        if (GunInventory.gunInventoryInstance.healItemAmount >= 1)
        {
            TDP.Heal(partId);
            GunInventory.gunInventoryInstance.healItemAmount--;
            hpText.VisibleText();        
        }
    }

    // ボタンから呼ばれるメソッド
    public void OnClickHead() => HealPart(1);
    public void OnClickChest() => HealPart(2);
    public void OnClickLeftArm() => HealPart(3);
    public void OnClickRightArm() => HealPart(4);
    public void OnClickLeftLeg() => HealPart(5);
    public void OnClickRightLeg() => HealPart(6);
}
