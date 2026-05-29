using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TakeDamagePlayer : MonoBehaviour
{
    [Header("プレイヤーの部位ごとのHP")]
    [Tooltip("頭のHP")]
    public int headHP = 5;
    [Tooltip("胴体のHP")]
    public int chestHP = 15;
    [Tooltip("左足のHP")]
    public int leftLegHP = 5;
    [Tooltip("右足のHP")]
    public int rightLegHP = 5;
    [Tooltip("左手のHP")]
    public int leftArmHP = 5;
    [Tooltip("右手のHP")]
    public int rightArmHP = 5;

    private int damagePoint; //ダメージを受ける部位
    private int damageArmPoint;　//どっちの腕にダメージを与えるか決める
    private int damageLegPoint; //どっちの足にダメージを与えるか決める

    [SerializeField]
    private GameObject[] bloodEffect;

    [Tooltip("0:頭をセット,1:胴体をセット,2:左手をセット,3右手をセット,4:左足をセット,5:右足をセット")]
    public GameObject[] bodyUI;
    [Tooltip("0:頭をセット,1:胴体をセット,2:左手をセット,3右手をセット,4:左足をセット,5:右足をセット")]
    public GameObject[] damagedBodyUI;

    private PlayerMovementScript PMS;
    private GameObject player;
    private WhichDeadBodyPart deadBodyPart;
    public Volume volume;
    public Vignette vignette;

    private void Start()
    {
        if (volume != null)
        {
            //vignetteを取得
            bool gotVignette = volume.profile.TryGet(out vignette);
            //Debug.Log($"TryGet Vignette: {gotVignette}");

            if (gotVignette)
            {
                vignette.active = true;
                vignette.intensity.value = 0f;
            }
            else
            {
                Debug.LogError("Vignette component not found in profile.");
            }

            //Vignetteの値を初期化
            if (vignette!=null)
            {
                vignette.active = true;
                vignette.intensity.value = 0;
            }
            else
            {
                Debug.LogError("Vignette が Volume Profile に追加されていません");
            }
        }
        else
        {
            Debug.LogWarning("volume is not set");
        }

        //プレイヤーをタグで検索
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PMS = player.GetComponent<PlayerMovementScript>();
            if (PMS == null)
            {
                Debug.LogWarning("Script Missing");
            }
        }
        else
        {
            Debug.LogWarning("PlayerObject is not found");
        }   
    }

   /* private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamage(5);
        }  
    }
*/
    private IEnumerator DestroyBloodEffect()
    {
        yield return new WaitForSeconds(1.5f);

        //すべての血のエフェクトを非表示
        for (int i = 0; i < 3; i++)
        {
            bloodEffect[i].SetActive(false);
        }
    }

    [SerializeField]
    private CameraShake cameraShake; //インスペクタで設定
    
    //プレイヤーがダメージを受けたとき
    public void TakeDamage(int damage)
    {
        PlayerMovementScript script = GetComponent<PlayerMovementScript>();
        if (script.enabled == true)
        {
            script.Damage();
        }
        
        // カメラを揺らす
        cameraShake.Shake(0.2f, 0.6f);  // 0.2秒間、強さ0.1で揺らす

        //血のエフェクトをランダムで表示
        int random = Random.Range(0, 3);

        bloodEffect[random].SetActive(true);
        StartCoroutine("DestroyBloodEffect");

        List<int> damageableParts = new List<int>();

        // 各部位のHPをチェックして、生きてる部位だけリストに追加
        if (headHP > 0) damageableParts.Add(0);
        if (leftArmHP > 0 || rightArmHP > 0) damageableParts.Add(1);
        if (leftLegHP > 0 || rightLegHP > 0) damageableParts.Add(2);
        if (chestHP > 0) damageableParts.Add(3);

        // ダメージ可能な部位がないなら処理を終える
        if (damageableParts.Count == 0)
        {
            Debug.Log("すべての部位が破壊されている！");
            return;
        }

        // ランダムに部位を選択
        int selectedPart = damageableParts[Random.Range(0, damageableParts.Count)];
        int damagePart = 0;

        switch (selectedPart)
        {
            case 0: // 頭
                Debug.LogWarning("頭にダメージを受けた！");
                headHP -= damage;
                headHP = Mathf.Max(headHP, 0);
                damagePart = 1;
                break;

            case 1: // 腕
                damageArmPoint = Random.Range(0, 2);
                if (damageArmPoint == 0 && leftArmHP > 0)
                {
                    Debug.LogWarning("左手にダメージを受けた！");
                    leftArmHP -= damage;
                    leftArmHP = Mathf.Max(leftArmHP, 0);
                    damagePart = 2;
                }
                else if (rightArmHP > 0)
                {
                    Debug.LogWarning("右手にダメージを受けた！");
                    rightArmHP -= damage;
                    rightArmHP = Mathf.Max(rightArmHP, 0);
                    damagePart = 3;
                }
                break;

            case 2: // 足
                damageLegPoint = Random.Range(0, 2);
                if (damageLegPoint == 0 && leftLegHP > 0)
                {
                    Debug.LogWarning("左足にダメージを受けた！");
                    leftLegHP -= damage;
                    leftLegHP = Mathf.Max(leftLegHP, 0);
                    damagePart = 4;
                }
                else if (rightLegHP > 0)
                {
                    Debug.LogWarning("右足にダメージを受けた！");
                    rightLegHP -= damage;
                    rightLegHP = Mathf.Max(rightLegHP, 0);
                    damagePart = 5;
                }
                break;

            case 3: // 胴体
                Debug.LogWarning("胴体にダメージを受けた！");
                chestHP -= damage;
                chestHP = Mathf.Max(chestHP, 0);
                damagePart = 6;
                break;
        }

        //いずれかの部位のHPが0になったとき
        int[] bodyPartsHP = { headHP, chestHP, leftArmHP, rightArmHP, leftLegHP, rightLegHP };

        if (bodyPartsHP.Any(hp => hp <= 0))
        {
            switch (damagePart)
            {
                case 1:
                    deadBodyPart = WhichDeadBodyPart.head;
                    DeBuff();
                    break;
                case 2:
                    deadBodyPart = WhichDeadBodyPart.leftArm;
                    DeBuff();
                    break;
                case 3:
                    deadBodyPart = WhichDeadBodyPart.rightArm;
                    DeBuff();
                    break;
                case 4:
                    deadBodyPart = WhichDeadBodyPart.leftLeg;
                    DeBuff();
                    break;
                case 5:
                    deadBodyPart = WhichDeadBodyPart.rightLeg;
                    DeBuff();
                    break;
                case 6:
                    deadBodyPart = WhichDeadBodyPart.chest;
                    DeBuff();
                    break;
            }
        }

        Debug.Log($"headHP:{headHP}\n " +
                      $"chestHP:{chestHP}\n " +
                      $"leftHP{leftArmHP}\n " +
                      $"rightHP:{rightArmHP}\n " +
                      $"leftHP{leftLegHP}\n " +
                      $"rightHP{rightLegHP}");
    }

    public int healValue; //回復量

    //体の部位を回復
    //healPart 1(頭),　2(胴体),　3(左手),　4(右手),　5(左足),　6(右足)
    public void Heal(int healPert)
    {
        GameObject currentGun = GunInventory.gunInventoryInstance.currentGun;
        GunScript gunScript = currentGun.GetComponent<GunScript>();

        switch (healPert)
        {
            //頭を回復
            case 1:
                Debug.LogWarning("頭を回復");
                headHP += healValue;

                //体のUIを表示
                bodyUI[0].SetActive(true);

                //vignetteの量を0；
                vignette.intensity.value = 0;
                break;

            //胴体を回復
            case 2:
                Debug.LogWarning("胴体を回復");
                chestHP += healValue;

                bodyUI[1].SetActive(true);
                break;

            //左手を回復
            case 3:
                Debug.LogWarning("左手を回復");
                leftArmHP += healValue;

                //回復済みならステータス変更しない
                if (WasLarmInjured == true)
                {
                    //反動を減らす
                    gunScript.recoilAmount_x_non /= 1.5f;
                    gunScript.recoilAmount_x_ /= 1.5f;
                    gunScript.recoilAmount_y_non /= 1.5f;
                    gunScript.recoilAmount_y_ /= 1.5f;
                }
                bodyUI[2].SetActive(true);
                break;

            //右手を回復
            case 4:
                Debug.LogWarning("右手を回復");
                rightArmHP += healValue;

                //回復済みならステータス変更しない
                if (WasRarmInjured == true)
                {
                    //反動を減らす
                    gunScript.recoilAmount_x_non /= 1.5f;
                    gunScript.recoilAmount_x_ /= 1.5f;
                    gunScript.recoilAmount_y_non /= 1.5f;
                    gunScript.recoilAmount_y_ /= 1.5f;
                }
                bodyUI[3].SetActive(true);
                break;

            //左足を回復
            case 5:
                Debug.LogWarning("左足を回復");
                leftLegHP += healValue;

                //回復済みならステータス変更しない
                if (WasLeftLegInjured == true)
                {
                    //移動速度、ジャンプ力を増加
                    PMS.accelerationSpeed /= 0.8f;              
                    WasLeftLegInjured = false;
                }
                bodyUI[4].SetActive(true);
                break;

            //右足を回復
            case 6:
                Debug.LogWarning("右足を回復");
                rightLegHP += healValue;

                //回復済みならステータス変更しない
                if (WasRightLegInjured == true)
                {
                    //移動速度、ジャンプ力を増加
                    PMS.accelerationSpeed /= 0.8f;
                    WasRightLegInjured = false;
                }
                bodyUI[5].SetActive(true);
                break;
        }
    }

    //プレイヤーの移動速度を一度だけ変更するため
    private bool WasLeftLegInjured;
    private bool WasRightLegInjured;
    private bool WasLarmInjured;
    private bool WasRarmInjured;

    //プレイヤーにデバフを与える
    public void DeBuff()
    {
        GameObject currentGun = GunInventory.gunInventoryInstance.currentGun;
        GunScript gunScript = currentGun.GetComponent<GunScript>();
        int Part = (int)deadBodyPart;
       
        switch (deadBodyPart)
        {
            //頭を負傷すると視界悪化
            case WhichDeadBodyPart.head:
                if (headHP <= 0)
                {
                    Debug.LogWarning("視界悪化");                   
                    vignette.intensity.value = 0.8f;
                    bodyUI[Part].SetActive(false);
                }
                break;

            //胸のHPが0になるとゲームオーバー
            case WhichDeadBodyPart.chest:
                if (chestHP <= 0)
                {
                    Debug.LogWarning("死亡");
                    PMS.Die();
                }               
                break;

            //手を負傷すると銃の反動増加
            case WhichDeadBodyPart.leftArm:
                if (leftArmHP <= 0)
                {
                    Debug.LogWarning("反動増加");
                    bodyUI[Part].SetActive(false);
                    gunScript.recoilAmount_x_non *= 1.5f;
                    gunScript.recoilAmount_x_ *= 1.5f;
                    gunScript.recoilAmount_y_non *= 1.5f;
                    gunScript.recoilAmount_y_ *= 1.5f;

                    WasLarmInjured = true;
                }
                break;
            case WhichDeadBodyPart.rightArm:
                if (rightArmHP <= 0)
                {
                    Debug.LogWarning("反動増加");
                    bodyUI[Part].SetActive(false);
                    gunScript.recoilAmount_x_non *= 1.5f;
                    gunScript.recoilAmount_x_ *= 1.5f;
                    gunScript.recoilAmount_y_non *= 1.5f;
                    gunScript.recoilAmount_y_ *= 1.5f;

                    WasRarmInjured = true;
                }
                break;

            //足を負傷したときは移動速度を低下
            case WhichDeadBodyPart.leftLeg:
                if (WasLeftLegInjured == false)
                {
                    Debug.LogWarning("移動速度とジャンプ力を低下");
                    bodyUI[Part].SetActive(false);
                    PMS.accelerationSpeed *= 0.8f;
                    WasLeftLegInjured = true;
                }
                break;
            case WhichDeadBodyPart.rightLeg:
                if (WasRightLegInjured == false)
                {
                    Debug.LogWarning("移動速度とジャンプ力を低下");
                    bodyUI[Part].SetActive(false);
                    PMS.accelerationSpeed *= 0.8f;
                    WasRightLegInjured = true;
                }
                break;
        }
    }
}