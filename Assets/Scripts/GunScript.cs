using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;

//using UnityStandardAssets.ImageEffects;

//銃の射撃方法
public enum GunStyles{
	nonautomatic,　//単発
    automatic, //フルオート
    shotgun //散弾
}

//銃の種類
public enum WeaponType
{
    Pistol,
    MachineGun,
    ShotGun
}


[System.Serializable]
public class BulletData
{
    [Header("弾のプロパティ")]
	[Tooltip("Preset value to tell with how many bullets will our waepon spawn aside.")]

    //全体で所持している弾薬
	public float bulletsIHave = 20;
	[Tooltip("Preset value to tell with how much bullets will our waepon spawn inside rifle.")]

    //マガジンに入っている弾薬
	public float bulletsInTheGun = 5;

    public BulletData() { }

    public BulletData(BulletData other)
    {
        this.bulletsIHave = other.bulletsIHave;
        this.bulletsInTheGun = other.bulletsInTheGun;
    }
}

public class GunScript : MonoBehaviour {
	[Tooltip("Selects type of waepon to shoot rapidly or one bullet per click.")]
	public GunStyles currentStyle;
    public WeaponType weaponType;

	[HideInInspector]
	public MouseLookScript mls;

    //1マガジンにの最大装填数
    public int amountOfBulletsPerLoad = 5;

    //銃の種類ごとのデータ
    public BulletData pistolData = new BulletData();
    public BulletData machineGunData = new BulletData();
    public BulletData shotGunData = new BulletData();

    //銃の種類ごとにファイルにセーブ
    private string GetFileName(WeaponType type)
    {
        return Application.persistentDataPath + $"/bullet_{type.ToString().ToLower()}.json";
    }

    //弾の内容をセーブ
    public void SaveBulletData(WeaponType type)
    {
        BulletData info = GetBulletData(type);
        string json = JsonUtility.ToJson(info, true);
        File.WriteAllText(GetFileName(type), json);
      //  Debug.Log($"{type} 用のデータを保存しました。");
    }

    //弾の内容をロード
    public void LoadBulletData(WeaponType type)
    {
        string path = GetFileName(type);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            BulletData loaded = JsonUtility.FromJson<BulletData>(json);
            SetBulletData(type, loaded);
           // Debug.Log($"{type} 用のデータを読み込みました。");
        }
        else
        {
            Debug.LogWarning($"{type} のデータファイルが見つかりません: {path}");
        }
    }

    //弾のデータを取り出す
    public BulletData GetBulletData(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.Pistol: return pistolData;
            case WeaponType.MachineGun: return machineGunData;
            case WeaponType.ShotGun: return shotGunData;
            default: return null;
        }
    }

    //弾のデータを更新
    public void SetBulletData(WeaponType type, BulletData data)
    {
        switch (type)
        {
            case WeaponType.Pistol: pistolData = data; break;
            case WeaponType.MachineGun: machineGunData = data; break;
            case WeaponType.ShotGun: shotGunData = data; break;
        }
    }

    //装備している銃によって移動速度を変更
    [Header("プレイヤーのプロパティ")]
	[Tooltip("Speed is determined via gun because not every gun has same properties or weights so you MUST set up your speeds here")]

    //歩行スピード
	public int walkingSpeed = 3;
	[Tooltip("Speed is determined via gun because not every gun has same properties or weights so you MUST set up your speeds here")]

    //走行スピード
	public int runningSpeed = 5;

    private Transform player;
	private Camera cameraComponent;

	private PlayerMovementScript pmS;

    public static GunScript Instance { get; private set; }
	/*
	 * Collection the variables upon awake that we need.
	 */
	void Awake(){

        //インスタンスを設定
        if (Instance == null)
        {
            Instance = this;
           // Debug.Log("セット完了");
        }
        else
        {
            Destroy(gameObject);
        }

		mls = GameObject.FindGameObjectWithTag("Player").GetComponent<MouseLookScript>();
		player = mls.transform;
		mainCamera = mls.myCamera;
		secondCamera = GameObject.FindGameObjectWithTag("SecondCamera").GetComponent<Camera>();
		cameraComponent = mainCamera.GetComponent<Camera>();
		pmS = player.GetComponent<PlayerMovementScript>();

        //弾の発射位置を設定
		bulletSpawnPlace = GameObject.FindGameObjectWithTag("BulletSpawn");
		hitMarker = transform.Find ("hitMarkerSound").GetComponent<AudioSource> ();

        //視点の感度設定
		startLook = mouseSensitvity_notAiming;
		startAim = mouseSensitvity_aiming;
		startRun = mouseSensitvity_running;

		rotationLastY = mls.currentYRotation;
		rotationLastX= mls.currentCameraXRotation;

	}

    [HideInInspector]
	public Vector3 currentGunPosition;
	[Header("Gun Positioning")]
	[Tooltip("Vector 3 position from player SETUP for NON AIMING values")]
	public Vector3 restPlacePosition;
	[Tooltip("Vector 3 position from player SETUP for AIMING values")]
	public Vector3 aimPlacePosition;
	[Tooltip("Time that takes for gun to get into aiming stance.")]
	public float gunAimTime = 0.1f;

	[HideInInspector]
	public bool reloading;

	private Vector3 gunPosVelocity;
	private float cameraZoomVelocity;
	private float secondCameraZoomVelocity;

	private Vector2 gunFollowTimeVelocity;

	/*
	Update loop calling for methods that are descriped below where they are initiated.
	*/
	void Update(){

		Animations();

		GiveCameraScriptMySensitvity();

		PositionGun();

        //回復画面を表示している間は実行しない
        if (pmS.openHealTab != true)
        {
            Shooting();
            MeeleAttack();
            LockCameraWhileMelee();

            Sprint(); 


            CrossHairExpansionWhenWalking();
        }
	}

	void FixedUpdate(){
		RotationGun ();

		MeeleAnimationsStates ();

        //右クリックを押してaim
		if(Input.GetAxis("Fire2") != 0 && !reloading && !meeleAttack && currentStyle!=GunStyles.shotgun){
			gunPrecision = gunPrecision_aiming;
			recoilAmount_x = recoilAmount_x_;
			recoilAmount_y = recoilAmount_y_;
			recoilAmount_z = recoilAmount_z_;
			currentGunPosition = Vector3.SmoothDamp(currentGunPosition, aimPlacePosition, ref gunPosVelocity, gunAimTime);
			cameraComponent.fieldOfView = Mathf.SmoothDamp(cameraComponent.fieldOfView, cameraZoomRatio_aiming, ref cameraZoomVelocity, gunAimTime);
			secondCamera.fieldOfView = Mathf.SmoothDamp(secondCamera.fieldOfView, secondCameraZoomRatio_aiming, ref secondCameraZoomVelocity, gunAimTime);
		}
		//aimをしていないとき　
		else{
			gunPrecision = gunPrecision_notAiming;
			recoilAmount_x = recoilAmount_x_non;
			recoilAmount_y = recoilAmount_y_non;
			recoilAmount_z = recoilAmount_z_non;
			currentGunPosition = Vector3.SmoothDamp(currentGunPosition, restPlacePosition, ref gunPosVelocity, gunAimTime);
			cameraComponent.fieldOfView = Mathf.SmoothDamp(cameraComponent.fieldOfView, cameraZoomRatio_notAiming, ref cameraZoomVelocity, gunAimTime);
			secondCamera.fieldOfView = Mathf.SmoothDamp(secondCamera.fieldOfView, secondCameraZoomRatio_notAiming, ref secondCameraZoomVelocity, gunAimTime);
		}

	}

	[Header("Sensitvity of the gun")]
	[Tooltip("aimしていないときの銃の感度")]
	public float mouseSensitvity_notAiming = 10;
	//[HideInInspector]
	[Tooltip("aimしているときの銃の感度")]
	public float mouseSensitvity_aiming = 5;
	//[HideInInspector]
	[Tooltip("走行中の銃の感度")]
	public float mouseSensitvity_running = 4;

	void GiveCameraScriptMySensitvity(){
		mls.mouseSensitvity_notAiming = mouseSensitvity_notAiming;
		mls.mouseSensitvity_aiming = mouseSensitvity_aiming;
	}

	void CrossHairExpansionWhenWalking(){

		if(player.GetComponent<Rigidbody>().linearVelocity.magnitude > 1 && Input.GetAxis("Fire1") == 0){//ifnot shooting

			expandValues_crosshair += new Vector2(20, 40) * Time.deltaTime;
			if(player.GetComponent<PlayerMovementScript>().maxSpeed < runningSpeed){ //not running
				expandValues_crosshair = new Vector2(Mathf.Clamp(expandValues_crosshair.x, 0, 10), Mathf.Clamp(expandValues_crosshair.y,0,20));
				fadeout_value = Mathf.Lerp(fadeout_value, 1, Time.deltaTime * 2);
			}
			else{//running
				fadeout_value = Mathf.Lerp(fadeout_value, 0, Time.deltaTime * 10);
				expandValues_crosshair = new Vector2(Mathf.Clamp(expandValues_crosshair.x, 0, 20), Mathf.Clamp(expandValues_crosshair.y,0,40));
			}
		}
		else{//if shooting
			expandValues_crosshair = Vector2.Lerp(expandValues_crosshair, Vector2.zero, Time.deltaTime * 5);
			expandValues_crosshair = new Vector2(Mathf.Clamp(expandValues_crosshair.x, 0, 10), Mathf.Clamp(expandValues_crosshair.y,0,20));
			fadeout_value = Mathf.Lerp(fadeout_value, 1, Time.deltaTime * 2);

		}

	}

	void Sprint(){// Running();  so i can find it with CTRL + F
		if (Input.GetAxis ("Vertical") > 0 && Input.GetAxisRaw ("Fire2") == 0 && meeleAttack == false && Input.GetAxisRaw ("Fire1") == 0) {
			if (Input.GetKeyDown (KeyCode.LeftShift)) {
				if (pmS.maxSpeed == walkingSpeed) {
					pmS.maxSpeed = runningSpeed;//sets player movement peed to max

				} else {
					pmS.maxSpeed = walkingSpeed;
				}
			}
		} else {
			pmS.maxSpeed = walkingSpeed;
		}

	}

	[HideInInspector]
	public bool meeleAttack;
	[HideInInspector]
	public bool aiming;

	void MeeleAnimationsStates(){
		if (handsAnimator) {
			meeleAttack = handsAnimator.GetCurrentAnimatorStateInfo (0).IsName (meeleAnimationName);
			aiming = handsAnimator.GetCurrentAnimatorStateInfo (0).IsName (aimingAnimationName);	
		}
	}

	void MeeleAttack(){	

		if(Input.GetKeyDown(KeyCode.Q) && !meeleAttack){			
			StartCoroutine("AnimationMeeleAttack");
		}
	}

	IEnumerator AnimationMeeleAttack(){
		handsAnimator.SetBool("meeleAttack",true);
		//yield return new WaitForEndOfFrame();
		yield return new WaitForSeconds(0.1f);
		handsAnimator.SetBool("meeleAttack",false);
	}

	private float startLook, startAim, startRun;

	void LockCameraWhileMelee(){
		if (meeleAttack) {
			mouseSensitvity_notAiming = 2;
			mouseSensitvity_aiming = 1.6f;
			mouseSensitvity_running = 1;
		} else {
			mouseSensitvity_notAiming = startLook;
			mouseSensitvity_aiming = startAim;
			mouseSensitvity_running = startRun;
		}
	}


	private Vector3 velV;
	[HideInInspector]
	public Transform mainCamera;
	private Camera secondCamera;

	void PositionGun(){
		transform.position = Vector3.SmoothDamp(transform.position,
			mainCamera.transform.position  - 
			(mainCamera.transform.right * (currentGunPosition.x + currentRecoilXPos)) + 
			(mainCamera.transform.up * (currentGunPosition.y+ currentRecoilYPos)) + 
			(mainCamera.transform.forward * (currentGunPosition.z + currentRecoilZPos)),ref velV, 0);



		pmS.cameraPosition = new Vector3(currentRecoilXPos,currentRecoilYPos, 0);

		currentRecoilZPos = Mathf.SmoothDamp(currentRecoilZPos, 0, ref velocity_z_recoil, recoilOverTime_z);
		currentRecoilXPos = Mathf.SmoothDamp(currentRecoilXPos, 0, ref velocity_x_recoil, recoilOverTime_x);
		currentRecoilYPos = Mathf.SmoothDamp(currentRecoilYPos, 0, ref velocity_y_recoil, recoilOverTime_y);

	}


	[Header("Rotation")]
	private Vector2 velocityGunRotate;
	private float gunWeightX,gunWeightY;
	[Tooltip("The time waepon will lag behind the camera view best set to '0'.")]
	public float rotationLagTime = 0f;
	private float rotationLastY;
	private float rotationDeltaY;
	private float angularVelocityY;
	private float rotationLastX;
	private float rotationDeltaX;
	private float angularVelocityX;
	[Tooltip("Value of forward rotation multiplier.")]
	public Vector2 forwardRotationAmount = Vector2.one;

	void RotationGun(){

		rotationDeltaY = mls.currentYRotation - rotationLastY;
		rotationDeltaX = mls.currentCameraXRotation - rotationLastX;

		rotationLastY= mls.currentYRotation;
		rotationLastX= mls.currentCameraXRotation;

		angularVelocityY = Mathf.Lerp (angularVelocityY, rotationDeltaY, Time.deltaTime * 5);
		angularVelocityX = Mathf.Lerp (angularVelocityX, rotationDeltaX, Time.deltaTime * 5);

		gunWeightX = Mathf.SmoothDamp (gunWeightX, mls.currentCameraXRotation, ref velocityGunRotate.x, rotationLagTime);
		gunWeightY = Mathf.SmoothDamp (gunWeightY, mls.currentYRotation, ref velocityGunRotate.y, rotationLagTime);

		transform.rotation = Quaternion.Euler (gunWeightX + (angularVelocityX*forwardRotationAmount.x), gunWeightY + (angularVelocityY*forwardRotationAmount.y), 0);
	}

	private float currentRecoilZPos;
	private float currentRecoilXPos;
	private float currentRecoilYPos;

     //銃の反動
	public void RecoilMath(){
		currentRecoilZPos -= recoilAmount_z;
		currentRecoilXPos -= (Random.value - 0.5f) * recoilAmount_x;
		currentRecoilYPos -= (Random.value - 0.5f) * recoilAmount_y;
		mls.wantedCameraXRotation -= Mathf.Abs(currentRecoilYPos * gunPrecision);
		mls.wantedYRotation -= (currentRecoilXPos * gunPrecision);		 

		expandValues_crosshair += new Vector2(6,12);

	}

	[Header("Shooting setup - MUSTDO")]
	[HideInInspector] public GameObject bulletSpawnPlace;
	[Tooltip("Bullet prefab that this waepon will shoot.")]
	public GameObject bullet;
	[Tooltip("Rounds per second if weapon is set to automatic rafal.")]
	public float roundsPerSecond;
	public float waitTillNextFire;
	void Shooting(){

		if (!meeleAttack) {

            //射撃方法が単発の場合
			if (currentStyle == GunStyles.nonautomatic) {
				if (Input.GetButtonDown ("Fire1")) {
                    //弾のデータを取得
                    LoadBulletData(weaponType);
                    GetBulletData(weaponType);
                    if (pistolData != null)
                    {
                        receiveBulletData(pistolData.bulletsInTheGun, pistolData.bulletsIHave);
                    }

					ShootMethod ();
				}
			}

            //射撃方法が散弾の場合
            if (currentStyle == GunStyles.shotgun)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    //弾のデータを取得
                    LoadBulletData(weaponType);
                    GetBulletData(weaponType);
                    if (shotGunData != null)
                    {
                        receiveBulletData(shotGunData.bulletsInTheGun, shotGunData.bulletsIHave);
                    }

                    ShootMethod();
                }
            }

            //射撃方法がフルオートの場合
            if (currentStyle == GunStyles.automatic) {
				if (Input.GetButton ("Fire1")) { 

                    //弾のデータを取得
                    LoadBulletData(weaponType);
                    GetBulletData(weaponType);

                    if (machineGunData != null)
                    {
                        receiveBulletData(machineGunData.bulletsInTheGun, machineGunData.bulletsIHave);
                    }

                    ShootMethod ();
				}
			}
        }
		waitTillNextFire -= roundsPerSecond * Time.deltaTime;
	}


	[HideInInspector]	public float recoilAmount_z = 0.5f;
	[HideInInspector]	public float recoilAmount_x = 0.5f;
	[HideInInspector]	public float recoilAmount_y = 0.5f;
	[Header("aimしていないときの銃の反動")]
	[Tooltip("Z軸の反動")]
	public float recoilAmount_z_non = 0.5f;
	[Tooltip("Y軸の反動")]
	public float recoilAmount_x_non = 0.5f;
	[Tooltip("Y軸の反動")]
	public float recoilAmount_y_non = 0.5f;
	[Header("aimしているときの銃の反動")]
	[Tooltip("Z軸の反動")]
	public float recoilAmount_z_ = 0.5f;
	[Tooltip("X軸の反動")]
	public float recoilAmount_x_ = 0.5f;
	[Tooltip("Y軸の反動")]
	public float recoilAmount_y_ = 0.5f;
	[HideInInspector]public float velocity_z_recoil,velocity_x_recoil,velocity_y_recoil;
	[Header("")]
	[Tooltip("銃が元の位置に戻るまでの時間")]
	public float recoilOverTime_z = 0.5f;
	[Tooltip("銃が元の位置に戻るまでの時間")]
	public float recoilOverTime_x = 0.5f;
	[Tooltip("銃が元の位置に戻るまでの時間")]
	public float recoilOverTime_y = 0.5f;

	public float gunPrecision_notAiming = 200.0f;
	public float gunPrecision_aiming = 100.0f;
	public float cameraZoomRatio_notAiming = 60;
	public float cameraZoomRatio_aiming = 40;
	public float secondCameraZoomRatio_notAiming = 60;
	public float secondCameraZoomRatio_aiming = 40;

	[HideInInspector]
	public float gunPrecision;
	public AudioSource shoot_sound_source, reloadSound_source;
	public static AudioSource hitMarker;

	public static void HitMarkerSound(){
		hitMarker.Play();
	}

	public GameObject[] muzzelFlash;
	public GameObject muzzelSpawn;
	private GameObject holdFlash;
	private GameObject holdSmoke;

    //発射するぺレットの数
    public int pelletCount = 12;
    //弾が広がる角度
    public float spreadAngle = 15f;

    private float receiveBulletsInTheGun;
    private float receiveBulletsIHave;

    //受け取ったデータを代入する
    public void receiveBulletData(float bulletsIntheGunData,float bulletsIHaveData)
    {
        receiveBulletsInTheGun = bulletsIntheGunData;
        receiveBulletsIHave = bulletsIHaveData;
    }


	//射撃をするときのメソッド
	private void ShootMethod()
	{

		if (waitTillNextFire <= 0 && !reloading && pmS.maxSpeed < 5)
		{

			//マガジンに弾が入っているとき
			if (receiveBulletsInTheGun > 0)
			{

				//ショットガンじゃない場合
				if (currentStyle != GunStyles.shotgun)
				{
					int randomNumberForMuzzelFlash = Random.Range(0, 5);
					if (bullet)
						Instantiate(bullet, bulletSpawnPlace.transform.position, bulletSpawnPlace.transform.rotation);
					else
						Debug.Log("Missing the bullet prefab");
					holdFlash = Instantiate(muzzelFlash[randomNumberForMuzzelFlash], muzzelSpawn.transform.position /*- muzzelPosition*/, muzzelSpawn.transform.rotation * Quaternion.Euler(0, 0, 90)) as GameObject;
					holdFlash.transform.parent = muzzelSpawn.transform;
					if (shoot_sound_source)
						shoot_sound_source.Play();
					else
						print("Missing 'Shoot Sound Source'.");

					RecoilMath();

					waitTillNextFire = 1;

					//弾数を減らし、弾数のデータを更新
					if (weaponType == WeaponType.Pistol)
					{
						pistolData.bulletsInTheGun--;
						SetBulletData(weaponType, pistolData);
						SaveBulletData(WeaponType.Pistol);
					}
					else if (weaponType == WeaponType.MachineGun)
					{
						machineGunData.bulletsInTheGun--;
						SetBulletData(weaponType, machineGunData);
						SaveBulletData(WeaponType.MachineGun);
					}
				}
				else
				{
					for (int i = 0; i < pelletCount; i++)
					{
						// 拡散角度をランダムに生成
						float spreadX = Random.Range(-spreadAngle, spreadAngle);
						float spreadY = Random.Range(-spreadAngle, spreadAngle);

						// 弾を生成
						int randomNumberForMuzzelFlash = Random.Range(0, 5);
						if (bullet)
							Instantiate(bullet, bulletSpawnPlace.transform.position, Quaternion.Euler(bulletSpawnPlace.transform.eulerAngles.x + spreadX, bulletSpawnPlace.transform.eulerAngles.y + spreadY, 0));
						else
							print("Missing the bullet prefab");
						holdFlash = Instantiate(muzzelFlash[randomNumberForMuzzelFlash], muzzelSpawn.transform.position /*- muzzelPosition*/, muzzelSpawn.transform.rotation * Quaternion.Euler(0, 0, 90)) as GameObject;
						holdFlash.transform.parent = muzzelSpawn.transform;


						RecoilMath();

						waitTillNextFire = 2;

					}
					//弾数を減らし、弾数のデータを更新
					if (weaponType == WeaponType.ShotGun)
					{
						shotGunData.bulletsInTheGun--;
						SetBulletData(weaponType, shotGunData);
						SaveBulletData(WeaponType.ShotGun);
					}

					if (shoot_sound_source)
						shoot_sound_source.Play();
					else
						print("Missing 'Shoot Sound Source'.");
				}
			}
			else
			{
				StartCoroutine("Reload_Animation");

			}

		}
	}

	public float reloadChangeBulletsTime;

    //銃をリロードするコルーチン
	IEnumerator Reload_Animation(){
		if(receiveBulletsIHave > 0 && receiveBulletsInTheGun < amountOfBulletsPerLoad && !reloading/* && !aiming*/){

			if (reloadSound_source.isPlaying == false && reloadSound_source != null) {
				if (reloadSound_source)
					reloadSound_source.Play ();
				else
					print ("'Reload Sound Source' missing.");
			}
		
			handsAnimator.SetBool("reloading",true);
			yield return new WaitForSeconds(0.5f);
			handsAnimator.SetBool("reloading",false);



			yield return new WaitForSeconds (reloadChangeBulletsTime - 0.5f);//minus ovo vrijeme cekanja na yield
			if (meeleAttack == false && pmS.maxSpeed != runningSpeed) {
				if (receiveBulletsIHave - amountOfBulletsPerLoad >= 0) {
					receiveBulletsIHave -= amountOfBulletsPerLoad - receiveBulletsInTheGun; //全体で所持している弾薬からリロードした分を引く
					receiveBulletsInTheGun = amountOfBulletsPerLoad;　//マガジン内の弾薬を更新する
                    DataUpDateOnReload(receiveBulletsInTheGun, receiveBulletsIHave);
                 
				} else if (receiveBulletsIHave - amountOfBulletsPerLoad < 0) {
					float valueForBoth = amountOfBulletsPerLoad - receiveBulletsInTheGun;
					if (receiveBulletsIHave - valueForBoth < 0) {
						receiveBulletsInTheGun += receiveBulletsIHave;
						receiveBulletsIHave = 0;
                        DataUpDateOnReload(receiveBulletsInTheGun, receiveBulletsIHave);
                    } else {
						receiveBulletsIHave -= valueForBoth;
						receiveBulletsInTheGun += valueForBoth;
                        DataUpDateOnReload(receiveBulletsInTheGun, receiveBulletsIHave);
                    }
				}
			} else {
				reloadSound_source.Stop ();

				print ("Reload interrupted via meele attack");
			}

		}
	}

    //弾のデータを更新
    void DataUpDateOnReload(float bulletInTheGun, float bulletIHave )
    {
        switch (weaponType)
        {
            case WeaponType.Pistol:
                pistolData.bulletsIHave = bulletIHave;
                pistolData.bulletsInTheGun = bulletInTheGun; 
                SaveBulletData(weaponType);
                break;

            case WeaponType.MachineGun:
                Debug.Log($"{machineGunData.bulletsIHave}-{amountOfBulletsPerLoad}={machineGunData.bulletsIHave - amountOfBulletsPerLoad}");
                machineGunData.bulletsIHave = bulletIHave;
                machineGunData.bulletsInTheGun = bulletInTheGun;
                SaveBulletData(weaponType);
                break;

            case WeaponType.ShotGun:
                shotGunData.bulletsIHave = bulletIHave;
                shotGunData.bulletsInTheGun = bulletInTheGun;
                SaveBulletData(weaponType);
                break;
        }
    }

    //ゲーム画面に弾数の情報を表示
	[Tooltip("HUD bullets to display bullet count on screen. Will be find under name 'HUD_bullets' in scene.")]
	public TextMesh HUD_bullets;
	void OnGUI(){
		if(!HUD_bullets){
			try{
				HUD_bullets = GameObject.Find("HUD_bullets").GetComponent<TextMesh>();
			}
			catch(System.Exception ex){
				print("Couldnt find the HUD_Bullets ->" + ex.StackTrace.ToString());
			}
		}
        if (mls && HUD_bullets)
        {
            BulletData bulletData = GetBulletData(weaponType);
            HUD_bullets.text =bulletData.bulletsIHave.ToString() + " - " + bulletData.bulletsInTheGun.ToString();
        }
		DrawCrosshair();
	}

	public Texture horizontal_crosshair, vertical_crosshair;
	public Vector2 top_pos_crosshair, bottom_pos_crosshair, left_pos_crosshair, right_pos_crosshair;
	public Vector2 size_crosshair_vertical = new Vector2(1,1), size_crosshair_horizontal = new Vector2(1,1);
	[HideInInspector]
	public Vector2 expandValues_crosshair;
	private float fadeout_value = 1;

	void DrawCrosshair()
    {//回復画面を表示している間は実行しない
        if (pmS.openHealTab != true)
        {
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, fadeout_value);
            if (Input.GetAxis("Fire2") == 0)
            {//if not aiming draw
                GUI.DrawTexture(new Rect(vec2(left_pos_crosshair).x + position_x(-expandValues_crosshair.x) + Screen.width / 2, Screen.height / 2 + vec2(left_pos_crosshair).y, vec2(size_crosshair_horizontal).x, vec2(size_crosshair_horizontal).y), vertical_crosshair);//left
                GUI.DrawTexture(new Rect(vec2(right_pos_crosshair).x + position_x(expandValues_crosshair.x) + Screen.width / 2, Screen.height / 2 + vec2(right_pos_crosshair).y, vec2(size_crosshair_horizontal).x, vec2(size_crosshair_horizontal).y), vertical_crosshair);//right

                GUI.DrawTexture(new Rect(vec2(top_pos_crosshair).x + Screen.width / 2, Screen.height / 2 + vec2(top_pos_crosshair).y + position_y(-expandValues_crosshair.y), vec2(size_crosshair_vertical).x, vec2(size_crosshair_vertical).y), horizontal_crosshair);//top
                GUI.DrawTexture(new Rect(vec2(bottom_pos_crosshair).x + Screen.width / 2, Screen.height / 2 + vec2(bottom_pos_crosshair).y + position_y(expandValues_crosshair.y), vec2(size_crosshair_vertical).x, vec2(size_crosshair_vertical).y), horizontal_crosshair);//bottom
            }
        }
	}

	private float position_x(float var){
		return Screen.width * var / 100;
	}
	private float position_y(float var)
	{
		return Screen.height * var / 100;
	}

	private Vector2 vec2(Vector2 _vec2){
		return new Vector2(Screen.width * _vec2.x / 100, Screen.height * _vec2.y / 100);
	}

	public Animator handsAnimator;
	/*
	* 腕のアニメーションを再生
	*/
	void Animations(){

		if(handsAnimator){

			reloading = handsAnimator.GetCurrentAnimatorStateInfo(0).IsName(reloadAnimationName);

			handsAnimator.SetFloat("walkSpeed",pmS.currentSpeed);
			handsAnimator.SetBool("aiming", Input.GetButton("Fire2"));
			handsAnimator.SetInteger("maxSpeed", pmS.maxSpeed);
			if(Input.GetKeyDown(KeyCode.R) && pmS.maxSpeed < 5 && !reloading && !meeleAttack/* && !aiming*/){
				StartCoroutine("Reload_Animation");
			}
		}

	}

	[Header("Animation names")]
	public string reloadAnimationName = "Player_Reload";
	public string aimingAnimationName = "Player_AImpose";
	public string meeleAnimationName = "Character_Malee";
}
