using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {

	[Tooltip("Furthest distance bullet will look for target")]
	public float maxDistance = 1000000;
	RaycastHit hit;
	[Tooltip("Prefab of wall damange hit. The object needs 'LevelPart' tag to create decal on it.")]
	public GameObject decalHitWall;
	[Tooltip("Decal will need to be sligtly infront of the wall so it doesnt cause rendeing problems so for best feel put from 0.01-0.1.")]
	public float floatInfrontOfWall;
	[Tooltip("Blood prefab particle this bullet will create upoon hitting enemy")]
	public GameObject bloodEffect;
	[Tooltip("Put Weapon layer and Player layer to ignore bullet raycast.")]
	public LayerMask ignoreLayer;

    private int haveGun;

    [Header("銃のダメージ")]
    public int pistolDamage;
    public int machinegunDamage;
    public int shotGunDamage;
    private int bulletDamage;

    public static BulletScript instance;

    /*
	* Uppon bullet creation with this script attatched,
	* bullet creates a raycast which searches for corresponding tags.
	* If raycast finds somethig it will create a decal of corresponding tag.
	*/

    private void Awake()
    {
        //インスタンスを設定
        if (instance != null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        haveGun = GunInventory.gunInventoryInstance.currentGunCounter;
        Debug.Log(haveGun);
    }

    void Update () {

		if(Physics.Raycast(transform.position, transform.forward,out hit, maxDistance, ~ignoreLayer)){
            Debug.LogError(hit.transform.gameObject.layer);
			if(decalHitWall)
            {
                //敵にあたった時の処理と壁に当たった時の処理を追加
                if (hit.transform.gameObject.layer == 7 /*敵のレイヤーにそろえる*/) 
                { 
                    //敵にダメージを与える
                    MobStatus mobStatus = hit.transform.GetComponent<MobStatus>();
                    if (mobStatus == null)
                    {
                        mobStatus = hit.transform.GetComponentInParent<MobStatus>();
                    }

                    ChasePlayer chasePlayer = hit.transform.GetComponent<ChasePlayer>();
                    if (chasePlayer == null)
                    {
                        chasePlayer = hit.transform.GetComponentInParent<ChasePlayer>();
                    }

                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    switch (haveGun)
                    {
                        //ピストルのダメージ
                        case 0:
                            bulletDamage = pistolDamage;
                            break;
                        //マシンガンのダメージ
                        case 1:
                            bulletDamage = machinegunDamage;
                            break;
                        //ショットガンのダメージ
                        case 2:
                            bulletDamage = shotGunDamage;
                            break;
                        default:
                            break;
                    }

                   
                    if (mobStatus != null)
                    {
                        mobStatus.Damage(bulletDamage);
                        chasePlayer.ShotPlayer();
                    }
                    else
                    {
                        Debug.LogWarning("攻撃できない");
                    }

                    Destroy(gameObject);
                }

			}		
			Destroy(gameObject);
		}
		Destroy(gameObject, 0.1f);
	}

}
