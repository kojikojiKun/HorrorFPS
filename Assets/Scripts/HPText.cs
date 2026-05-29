using UnityEngine;
using TMPro;

public class HPText : MonoBehaviour
{
    private int healItemAmount;
    private int headHP;
    private int chestHP;
    private int leftArmHP;
    private int rightArmHP;
    private int leftLegHP;
    private int rightLegHP;

    public GameObject DamageController;
    public TextMeshProUGUI hpText;

    private void Start()
    {
        assignHP();
        hpText.text = "ITEM/" + healItemAmount + "\n\n"
                    + "HEAD/ " + headHP + "\n\n"
                    + "CHEST/ " + chestHP + "\n\n"
                    + "ARM_L/ " + leftArmHP + "\n\n"
                    + "ARM_R/ " + rightArmHP + "\n\n"
                    + "LEG_L/ " + leftLegHP + "\n\n"
                    + "LEG_R/ " + rightLegHP;
    }

    //部位の体力をtextで表示
    public void VisibleText()
    {
        assignHP();
        hpText.text = "ITEM/" + healItemAmount +"\n\n" 
                    + "HEAD/ " + headHP + "\n\n"
                    + "CHEST/ " + chestHP + "\n\n"
                    + "ARM_L/ " + leftArmHP + "\n\n"
                    + "ARM_R/ " + rightArmHP + "\n\n"
                    + "LEG_L/ " + leftLegHP + "\n\n"
                    + "LEG_R/ " + rightLegHP;
    }

    //部位ごとの体力をtextに代入
   　public void assignHP()
    {
        DamageController = GameObject.FindGameObjectWithTag("Player");
        TakeDamagePlayer takeDamagePlayer = DamageController.GetComponent<TakeDamagePlayer>();

        Debug.Log("アサイン完了");
        healItemAmount = GunInventory.gunInventoryInstance.healItemAmount;
        headHP = takeDamagePlayer.headHP;
        chestHP = takeDamagePlayer.chestHP;
        leftArmHP = takeDamagePlayer.leftArmHP;
        rightArmHP = takeDamagePlayer.rightArmHP;
        leftLegHP = takeDamagePlayer.leftLegHP;
        rightLegHP = takeDamagePlayer.rightLegHP;
    }
}
