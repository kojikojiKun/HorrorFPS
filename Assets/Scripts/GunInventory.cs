using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public enum MenuStyle{
	horizontal,vertical
}

public class GunInventory : MonoBehaviour
{
    [Tooltip("Current weapon gameObject.")]
    public GameObject currentGun;
    private Animator currentHAndsAnimator;
    public int currentGunCounter = 0;
    public static GunInventory gunInventoryInstance;

    [Tooltip("Put Strings of weapon objects from Resources Folder.")]
    public List<string> gunsIHave = new List<string>();
    [Tooltip("Icons from weapons.(Fetched when you run the game)*MUST HAVE ICONS WITH CORRESPONDING NAMES IN RESOUCES FOLDER*")]
    public Texture[] icons;

    [HideInInspector]
    public float switchWeaponCooldown;


    /*
	 * Calling the method that will update the icons of our guns if we carry any upon start.
	 * Also will spawn a weapon upon start.
	 */
    void Awake()
    {
        StartCoroutine("UpdateIconsFromResources");

        StartCoroutine("SpawnWeaponUponStart");//to start with a gun

        if (gunsIHave.Count == 0)
            print("No guns in the inventory");

        //インスタンスを設定
        if (gunInventoryInstance == null)
        {
            gunInventoryInstance = this;
        }
        else
        {
            Debug.LogWarning("gunInventoryInstance is not set");
        }


    }

    public BulletData pistolBullet;
    public BulletData machineGunBullet;
    public BulletData shotGunBullet;

    //銃の弾のデータを初期化
    async void Start()
    {
        //1秒待機(GunScriptの初期化待機のため)
        await Task.Delay(2500);
        //  Debug.Log("待機");

        GunScript.Instance.SetBulletData(WeaponType.Pistol, new BulletData(pistolBullet));
        GunScript.Instance.SetBulletData(WeaponType.MachineGun, new BulletData(machineGunBullet));
        GunScript.Instance.SetBulletData(WeaponType.ShotGun, new BulletData(shotGunBullet));

        GunScript.Instance.SaveBulletData(WeaponType.Pistol);
        GunScript.Instance.SaveBulletData(WeaponType.MachineGun);
        GunScript.Instance.SaveBulletData(WeaponType.ShotGun);
    }

    IEnumerator SpawnWeaponUponStart()
    {
        yield return new WaitForSeconds(0);
        StartCoroutine("Spawn", 0);
    }

    void Update()
    {
        switchWeaponCooldown += 1 * Time.deltaTime;
        if (switchWeaponCooldown > 1.2f && Input.GetKey(KeyCode.LeftShift) == false)
        {
            Create_Weapon();
        }
    }


    //銃のアイコンを表示
    IEnumerator UpdateIconsFromResources()
    {
        yield return new WaitForEndOfFrame();

        icons = new Texture[gunsIHave.Count];
        icons[0] = (Texture)Resources.Load("Weap_Icons/" + gunsIHave[0].ToString() + "_img");

        //マシンガンを拾った時だけマシンガンのアイコンを表示
        if (pickupedLifle == true)
        {
            icons[1] = (Texture)Resources.Load("Weap_Icons/" + gunsIHave[1].ToString() + "_img");
        }

        //ショットガンを拾った時だけショットガンのアイコンを表示
        if (pickupedShotGun == true)
        {
            icons[2] = (Texture)Resources.Load("Weap_Icons/" + gunsIHave[2].ToString() + "_img");
        }
    }

    //武器を拾ったかどうか
    private bool pickupedLifle = false;
    private bool pickupedShotGun = false;

    [Header("アイテムを拾ったときに増加する弾の数")]
    public int IncreasePistolBullet;
    public int IncreaseMachineGunBullet;
    public int IncreaseShotGunBullet;

    //持っている回復アイテムの数
    public int healItemAmount;

    private int pickUpBulletType;

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            switch (other.tag)
            {
                case "lifle":
                    pickupedLifle = true;
                    Debug.Log("マシンガンを拾った");
                    StartCoroutine("UpdateIconsFromResources");
                    break;

                case "shotgun":
                    pickupedShotGun = true;
                    Debug.Log("ショットガンを拾った");
                    StartCoroutine("UpdateIconsFromResources");
                    break;

                case "PistolBullet":
                    pickUpBulletType = 0;
                    PickUPItem(IncreasePistolBullet);
                    Debug.Log($"ピストルの弾{IncreasePistolBullet}発を拾った");
                    break;

                case "MachineGunBullet":
                    pickUpBulletType = 1;
                    PickUPItem(IncreaseMachineGunBullet);
                    Debug.Log($"マシンガンの弾{IncreaseMachineGunBullet}発を拾った");
                    break;

                case "ShotGunBullet":
                    pickUpBulletType = 2;
                    PickUPItem(IncreaseShotGunBullet);
                    Debug.Log($"ショットガンの弾{IncreaseShotGunBullet}発を拾った");
                    break;

                case "HealItem":
                    healItemAmount += 1;
                    break;
            }
        }
    }

    //所持している弾薬に拾った弾薬をたす
    public void PickUPItem(int IncreaseBullet)
    {

        switch (pickUpBulletType)
        {
            case 0:
                BulletData pistolData = GunScript.Instance.GetBulletData(WeaponType.Pistol);

                if (pistolData != null)
                {

                    pistolData.bulletsIHave += IncreaseBullet;

                    GunScript.Instance.SetBulletData(WeaponType.Pistol, pistolData);
                }
                GunScript.Instance.SaveBulletData(WeaponType.Pistol);
                GunScript.Instance.LoadBulletData(WeaponType.Pistol);
                Debug.Log($"ピストルの弾数{pistolData.bulletsIHave}" + $"　{pistolData.bulletsInTheGun}");

                break;
            case 1:
                BulletData machineGunData = GunScript.Instance.GetBulletData(WeaponType.MachineGun);

                if (machineGunData != null)
                {
                    machineGunData.bulletsIHave += IncreaseBullet;

                    GunScript.Instance.SetBulletData(WeaponType.MachineGun, machineGunData);
                }
                GunScript.Instance.SaveBulletData(WeaponType.MachineGun);
                GunScript.Instance.LoadBulletData(WeaponType.MachineGun);
                Debug.Log($"マシンガンの弾数{machineGunData.bulletsIHave}" + $"　{machineGunData.bulletsInTheGun}");

                break;
            case 2:
                BulletData shotGunData = GunScript.Instance.GetBulletData(WeaponType.ShotGun);

                if (shotGunData != null)
                {
                    shotGunData.bulletsIHave += IncreaseBullet;

                    GunScript.Instance.SetBulletData(WeaponType.ShotGun, shotGunData);
                }
                GunScript.Instance.SaveBulletData(WeaponType.ShotGun);
                GunScript.Instance.LoadBulletData(WeaponType.ShotGun);
                Debug.Log($"ショットガンの弾数{shotGunData.bulletsIHave}" + $"　{shotGunData.bulletsInTheGun}");

                break;
        }
    }

    void Create_Weapon()
    {
        //ピストルをスポーンさせる
        if (Input.GetKeyDown(KeyCode.Alpha1) && currentGunCounter != 0)
        {
            currentHAndsAnimator.SetBool("changingWeapon", true);
            Destroy(currentGun);

            switchWeaponCooldown = 0;
            Debug.Log(switchWeaponCooldown);

            currentGunCounter = 0;
            Debug.Log(currentGunCounter);
            StartCoroutine("Spawn", currentGunCounter);

        }

        //マシンガンをスポーンさせる
        if (pickupedLifle == true && Input.GetKeyDown(KeyCode.Alpha2) && currentGunCounter != 1)
        {
            currentHAndsAnimator.SetBool("changingWeapon", true);
            Destroy(currentGun);

            switchWeaponCooldown = 0;
            Debug.Log(switchWeaponCooldown);

            currentGunCounter = 1;
            Debug.Log(currentGunCounter);
            StartCoroutine("Spawn", currentGunCounter);

        }

        //ショットガンをスポーンさせる
        if (pickupedShotGun == true && Input.GetKeyDown(KeyCode.Alpha3) && currentGunCounter != 2)
        {
            currentHAndsAnimator.SetBool("changingWeapon", true);
            Destroy(currentGun);

            switchWeaponCooldown = 0;
            Debug.Log(switchWeaponCooldown);

            currentGunCounter = 2;
            Debug.Log(currentGunCounter);

            StartCoroutine("Spawn", currentGunCounter);

        }

    }

    IEnumerator Spawn(int _redniBroj)
    {
        if (weaponChanging != null)
            weaponChanging.Play();
        else
            print("Missing Weapon Changing music clip.");

        if (currentGun != null)
        {
            if (currentGun.name.Contains("Gun"))
            {
                yield return null;

                GameObject resource = (GameObject)Resources.Load(gunsIHave[_redniBroj].ToString());
                GunScript gunScript = resource.GetComponent<GunScript>();

                //オブジェクトを生成
                currentGun = (GameObject)Instantiate(resource, transform.position, /*gameObject.transform.rotation*/Quaternion.identity);
                AssignHandsAnimator(currentGun);

                TakeDamagePlayer TDP = GetComponent<TakeDamagePlayer>();
                if (TDP != null)
                {
                    TDP.DeBuff();
                }
                else
                {
                    Debug.LogWarning("null");
                }

                switch (gunScript.weaponType)
                {
                    case WeaponType.Pistol:
                        //ピストルのデータを読み込む
                        GunScript.Instance.LoadBulletData(WeaponType.Pistol);
                        BulletData pistolData = GunScript.Instance.GetBulletData(WeaponType.Pistol);
                        if (pistolData != null)
                        {
                            //全体の弾数と弾倉に入っている弾数を渡す
                            GunScript.Instance.receiveBulletData(pistolData.bulletsInTheGun, pistolData.bulletsIHave);
                            Debug.Log($"ピストルの弾数{pistolData.bulletsIHave}" + $"　{pistolData.bulletsInTheGun}");
                        }
                        break;
                    case WeaponType.MachineGun:
                        //マシンガンのデータを読み込む
                        GunScript.Instance.LoadBulletData(WeaponType.MachineGun);
                        BulletData machineGunData = GunScript.Instance.GetBulletData(WeaponType.MachineGun);
                        if (machineGunData != null)
                        {
                            //全体の弾数と弾倉に入っている弾数を渡す
                            GunScript.Instance.receiveBulletData(machineGunData.bulletsInTheGun, machineGunData.bulletsIHave);
                            Debug.Log($"マシンガンの弾数{machineGunData.bulletsIHave}" + $"　{machineGunData.bulletsInTheGun}");
                        }
                        break;
                    case WeaponType.ShotGun:
                        //ショットガンのデータを読み込む
                        GunScript.Instance.LoadBulletData(WeaponType.ShotGun);
                        BulletData shotGunData = GunScript.Instance.GetBulletData(WeaponType.ShotGun);
                        if (shotGunData != null)
                        {
                            //全体の弾数と弾倉に入っている弾数を渡す
                            GunScript.Instance.receiveBulletData(shotGunData.bulletsInTheGun, shotGunData.bulletsIHave);
                            Debug.Log($"ショットガンの弾数{shotGunData.bulletsIHave}" + $"　{shotGunData.bulletsInTheGun}");
                        }
                        break;
                    default:
                        Debug.Log("不明のタイプです");
                        break;
                }
                Debug.Log($"WeaponType:{gunScript.weaponType}");
                Debug.Log($"CurrentGun:{currentGun}");
            }
        }
        else
        {
            GameObject resource = (GameObject)Resources.Load(gunsIHave[_redniBroj].ToString());
            GunScript gunScript = resource.GetComponent<GunScript>();
            currentGun = (GameObject)Instantiate(resource, transform.position, /*gameObject.transform.rotation*/Quaternion.identity);
            AssignHandsAnimator(currentGun);

            if (gunScript.weaponType == WeaponType.Pistol)
            {
                yield return new WaitForSeconds(1f);
                //ピストルのデータを読み込む
                GunScript.Instance.LoadBulletData(WeaponType.Pistol);
                BulletData pistolData = GunScript.Instance.GetBulletData(WeaponType.Pistol);
                if (pistolData != null)
                {
                    //全体の弾数と弾倉に入っている弾数を渡す
                    GunScript.Instance.receiveBulletData(pistolData.bulletsInTheGun, pistolData.bulletsIHave);
                    Debug.Log($"ピストルの弾数{pistolData.bulletsIHave}" + $"　{pistolData.bulletsInTheGun}");
                }
            }

            Debug.Log($"WeaponType:{gunScript.weaponType}");
            Debug.Log($"CurrentGun:{currentGun}");
        }
    }


    /*
	* Assigns Animator to the script so we can use it in other scripts of a current gun.
	*/
    void AssignHandsAnimator(GameObject _currentGun)
    {
        if (_currentGun.name.Contains("Gun"))
        {
            currentHAndsAnimator = currentGun.GetComponent<GunScript>().handsAnimator;
        }
    }

    /*
	 * Unity buil-in method to draw GUI.
	 * From here I am listing thourhg guns I have and drawing corresponding images on the sceen.
	 */
    void OnGUI()
    {
        if (currentGun)
        {
            for (int i = 0; i < gunsIHave.Count; i++)
            {
                DrawCorrespondingImage(i);
            }
        }

    }

    [Header("GUI Gun preview variables")]
    [Tooltip("Weapon icons style to pick.")]
    public MenuStyle menuStyle = MenuStyle.horizontal;
    [Tooltip("Spacing between icons.")]
    public int spacing = 10;
    [Tooltip("Begin position in percetanges of screen.")]
    public Vector2 beginPosition;
    [Tooltip("Size of icon in percetanges of screen.")]
    public Vector2 size;
    /*
	 * Passing the image number and gun list have the same sort,
	 * so it will fitthe gun image to our current gun or guns we have.
	 * The curent gun selected image has their image slightly enlared for some value.
	 */
    void DrawCorrespondingImage(int _number)
    {

        string deleteCloneFromName = currentGun.name.Substring(0, currentGun.name.Length - 7);

        if (icons[_number] != null)
        {
            if (menuStyle == MenuStyle.horizontal)
            {
                if (deleteCloneFromName == gunsIHave[_number])
                {
                    GUI.DrawTexture(new Rect(vec2(beginPosition).x + (_number * position_x(spacing)), vec2(beginPosition).y,//position variables
                        vec2(size).x, vec2(size).y),//size
                        icons[_number]);
                }
                else
                {
                    GUI.DrawTexture(new Rect(vec2(beginPosition).x + (_number * position_x(spacing) + 10), vec2(beginPosition).y + 10,//position variables
                        vec2(size).x - 20, vec2(size).y - 20),//size
                        icons[_number]);
                }
            }
            else if (menuStyle == MenuStyle.vertical)
            {
                if (deleteCloneFromName == gunsIHave[_number])
                {
                    GUI.DrawTexture(new Rect(vec2(beginPosition).x, vec2(beginPosition).y + (_number * position_y(spacing)),//position variables
                        vec2(size).x, vec2(size).y),//size
                        icons[_number]);
                }
                else
                {
                    GUI.DrawTexture(new Rect(vec2(beginPosition).x, vec2(beginPosition).y + 10 + (_number * position_y(spacing)),//position variables
                        vec2(size).x - 20, vec2(size).y - 20),//size
                        icons[_number]);
                }
            }

        }

    }

    /*
	 * プレイヤーが死亡した時に呼ばれる
	 */
    public void DeadMethod()
    {
        Destroy(currentGun);
    }


    //#####		RETURN THE SIZE AND POSITION for GUI images
    //(we pass in the percentage and it returns some number to appear in that percentage on the sceen) ##################
    private float position_x(float var)
    {
        return Screen.width * var / 100;
    }
    private float position_y(float var)
    {
        return Screen.height * var / 100;
    }
    private float size_x(float var)
    {
        return Screen.width * var / 100;
    }
    private float size_y(float var)
    {
        return Screen.height * var / 100;
    }
    private Vector2 vec2(Vector2 _vec2)
    {
        return new Vector2(Screen.width * _vec2.x / 100, Screen.height * _vec2.y / 100);
    }
    //######################################################

    /*
	 * Sounds
	 */
    [Header("Sounds")]
    [Tooltip("Sound of weapon changing.")]
    public AudioSource weaponChanging;
}
