using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

//負傷した部位
public enum WhichDeadBodyPart
{
    head,
    chest,
    leftArm,
    rightArm,
    leftLeg,
    rightLeg
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementScript : MonoBehaviour {
	//参照
	Rigidbody rb;
public GameObject HealBodyUI;
    public TextMeshProUGUI bodyHP;

	[Tooltip("Current players speed")]
	public float currentSpeed;
	[Tooltip("Assign players camera here")]
	[HideInInspector]public Transform cameraMain;
	[Tooltip("Force that moves player into jump")]
	public float jumpForce = 500;
	[Tooltip("Position of the camera inside the player")]
	[HideInInspector]public Vector3 cameraPosition;
    /*
	 * Getting the Players rigidbody component.
	 * And grabbing the mainCamera from Players child transform.
	 */

	void Awake(){
		rb = GetComponent<Rigidbody>();
		cameraMain = transform.Find("Main Camera").transform;
		bulletSpawn = cameraMain.Find ("BulletSpawn").transform;
		ignoreLayer = 1 << LayerMask.NameToLayer ("Player");

	}
	private Vector3 slowdownV;
	private Vector2 horizontalMovement;
	/*
	* Raycasting for meele attacks and input movement handling here.
	*/
	void FixedUpdate(){
		RaycastForMeleeAttacks ();

		PlayerMovementLogic ();
	}

    private float defaultJumpForce;
    private float defauletaccelerationSpeed;

    //ジャンプ力と移動スピードを初期化
    private void Start()
    {
        defaultJumpForce = jumpForce;
        defauletaccelerationSpeed = accelerationSpeed;

        //体力のテキスト非表示
        bodyHP.enabled = false;

        originalColor = gameOverText.color;
        // 最初は透明にする
        Color c = originalColor;
        c.a = 0f;
        gameOverText.color = c;
    }

    /*
	* Accordingly to input adds force and if magnitude is bigger it will clamp it.
	* If player leaves keys it will deaccelerate
	*/
    void PlayerMovementLogic(){
		currentSpeed = rb.linearVelocity.magnitude;
		horizontalMovement = new Vector2 (rb.linearVelocity.x, rb.linearVelocity.z);
		if (horizontalMovement.magnitude > maxSpeed){
			horizontalMovement = horizontalMovement.normalized;
			horizontalMovement *= maxSpeed;    
		}
		rb.linearVelocity = new Vector3 (
			horizontalMovement.x,
			rb.linearVelocity.y,
			horizontalMovement.y
		);
		if (grounded){
			rb.linearVelocity = Vector3.SmoothDamp(rb.linearVelocity,
				new Vector3(0,rb.linearVelocity.y,0),
				ref slowdownV,
				deaccelerationSpeed);
		}

		if (grounded) {
			rb.AddRelativeForce (Input.GetAxis ("Horizontal") * accelerationSpeed * Time.deltaTime, 0, Input.GetAxis ("Vertical") * accelerationSpeed * Time.deltaTime);
		} else {
			rb.AddRelativeForce (Input.GetAxis ("Horizontal") * accelerationSpeed / 2 * Time.deltaTime, 0, Input.GetAxis ("Vertical") * accelerationSpeed / 2 * Time.deltaTime);

		}
		/*
		 * Slippery issues fixed here
		 */
		if (Input.GetAxis ("Horizontal") != 0 || Input.GetAxis ("Vertical") != 0) {
			deaccelerationSpeed = 0.5f;
		} else {
			deaccelerationSpeed = 0.1f;
		}
	}
	/*
	* Handles jumping and ads the force and sounds.
	*/

    //スペースキーでジャンプ
	void Jumping(){
		if (Input.GetKeyDown (KeyCode.Space) && grounded) {
			rb.AddRelativeForce (Vector3.up * jumpForce);
			_walkSound.Stop ();
			_runSound.Stop ();
		}
	}
    /*
	* Update loop calling other stuff
	*/

    public GameObject takeDamagePlayer;
	void Update(){

		Jumping ();

		Crouching();

		WalkingSound ();

        Heal();

	}//end update

    
    public bool openHealTab=false;

    //回復を行うUIの表示非表示を切り替え
    void Heal()
    {
        if (HealBodyUI != null)
        {
            //TABキーを押すとUIを表示
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (openHealTab == false)
                {
                    Debug.Log("open heal");

                    //UIを表示する
                    HealBodyUI.SetActive(true);
                    Escape.instanse.textObj.enabled = true;

                    //テキストを表示
                    bodyHP.enabled = true;
                    Escape.instanse.Text();

                    HPText text = bodyHP.GetComponent<HPText>();
                    text.VisibleText();
                    openHealTab = true;

                    //マウスカーソル表示
                    Cursor.lockState = CursorLockMode.Confined;
                }
                else
                {
                    //もう一度TABキーを押すとUIを非表示
                    Debug.Log("close heal");
                    HealBodyUI.SetActive(false);
                    Escape.instanse.textObj.enabled = false;

                    //テキストを非表示
                    bodyHP.enabled = false;
                    openHealTab = false;

                    //マウスカーソル非表示
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }
        else
        {
            Debug.LogWarning("not found");
        }
    }

	/*
	* Checks if player is grounded and plays the sound accorindlgy to his speed
	*/
	void WalkingSound(){
		if (_walkSound && _runSound) {
			if (RayCastGrounded ()) { //for walk sounsd using this because suraface is not straigh			
				if (currentSpeed > 1) {
					//				print ("unutra sam");
					if (maxSpeed == 3) {
						//	print ("tu sem");
						if (!_walkSound.isPlaying) {
							//	print ("playam hod");
							_walkSound.Play ();
							_runSound.Stop ();
						}					
					} else if (maxSpeed == 5) {
						//	print ("NE tu sem");

						if (!_runSound.isPlaying) {
							_walkSound.Stop ();
							_runSound.Play ();
						}
					}
				} else {
					_walkSound.Stop ();
					_runSound.Stop ();
				}
			} else {
				_walkSound.Stop ();
				_runSound.Stop ();
			}
		} else {
			print ("Missing walk and running sounds.");
		}

	}
	/*
	* Raycasts down to check if we are grounded along the gorunded method() because if the
	* floor is curvy it will go ON/OFF constatly this assures us if we are really grounded
	*/
	private bool RayCastGrounded(){
		RaycastHit groundedInfo;
		if(Physics.Raycast(transform.position, transform.up *-1f, out groundedInfo, 1, ~ignoreLayer)){
			Debug.DrawRay (transform.position, transform.up * -1f, Color.red, 0.0f);
			if(groundedInfo.transform != null){
				//print ("vracam true");
				return true;
			}
			else{
				//print ("vracam false");
				return false;
			}
		}
		//print ("nisam if dosao");

		return false;
	}

	/*
	* If player toggle the crouch it will scale the player to appear that is crouching
	*/

        //Cキーでしゃがむ
	void Crouching(){
		if(Input.GetKey(KeyCode.C)){
			transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1,0.6f,1), Time.deltaTime * 15);
		}
		else{
			transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1,1,1), Time.deltaTime * 15);

		}
	}


	[Tooltip("The maximum speed you want to achieve")]
	public int maxSpeed = 5;
	[Tooltip("The higher the number the faster it will stop")]
	public float deaccelerationSpeed = 15.0f;


	[Tooltip("Force that is applied when moving forward or backward")]
	public float accelerationSpeed = 50000.0f;


	[Tooltip("Tells us weather the player is grounded or not.")]
	public bool grounded;
	/*
	* checks if our player is contacting the ground in the angle less than 60 degrees
	*	if it is, set groudede to true
	*/
	void OnCollisionStay(Collision other){
		foreach(ContactPoint contact in other.contacts){
			if(Vector2.Angle(contact.normal,Vector3.up) < 60){
				grounded = true;
			}
		}
	}
	/*
	* On collision exit set grounded to false
	*/
	void OnCollisionExit ()
	{
		grounded = false;
	}


	RaycastHit hitInfo;
	private float meleeAttack_cooldown;
	[Tooltip("Put 'Player' layer here")]
	[Header("Shooting Properties")]
	private LayerMask ignoreLayer;//to ignore player layer
	Ray ray1, ray2, ray3, ray4, ray5, ray6, ray7, ray8, ray9;
	private float rayDetectorMeeleSpace = 0.15f;
	private float offsetStart = 0.05f;
	[Tooltip("Put BulletSpawn gameobject here, palce from where bullets are created.")]
	[HideInInspector]
	public Transform bulletSpawn; //from here we shoot a ray to check where we hit him;
	/*
	* This method casts 9 rays in different directions. ( SEE scene tab and you will see 9 rays differently coloured).
	* Used to widley detect enemy infront and increase meele hit detectivity.
	* Checks for cooldown after last preformed meele attack.
	*/


	public bool been_to_meele_anim = false;
	private void RaycastForMeleeAttacks()
	{

		if (meleeAttack_cooldown > -5)
		{
			meleeAttack_cooldown -= 1 * Time.deltaTime;
		}

		if (GetComponent<GunInventory>().currentGun)
		{

			//middle row
			ray1 = new Ray(bulletSpawn.position + (bulletSpawn.right * offsetStart), bulletSpawn.forward + (bulletSpawn.right * rayDetectorMeeleSpace));
			ray2 = new Ray(bulletSpawn.position - (bulletSpawn.right * offsetStart), bulletSpawn.forward - (bulletSpawn.right * rayDetectorMeeleSpace));
			ray3 = new Ray(bulletSpawn.position, bulletSpawn.forward);
			//upper row
			ray4 = new Ray(bulletSpawn.position + (bulletSpawn.right * offsetStart) + (bulletSpawn.up * offsetStart), bulletSpawn.forward + (bulletSpawn.right * rayDetectorMeeleSpace) + (bulletSpawn.up * rayDetectorMeeleSpace));
			ray5 = new Ray(bulletSpawn.position - (bulletSpawn.right * offsetStart) + (bulletSpawn.up * offsetStart), bulletSpawn.forward - (bulletSpawn.right * rayDetectorMeeleSpace) + (bulletSpawn.up * rayDetectorMeeleSpace));
			ray6 = new Ray(bulletSpawn.position + (bulletSpawn.up * offsetStart), bulletSpawn.forward + (bulletSpawn.up * rayDetectorMeeleSpace));
			//bottom row
			ray7 = new Ray(bulletSpawn.position + (bulletSpawn.right * offsetStart) - (bulletSpawn.up * offsetStart), bulletSpawn.forward + (bulletSpawn.right * rayDetectorMeeleSpace) - (bulletSpawn.up * rayDetectorMeeleSpace));
			ray8 = new Ray(bulletSpawn.position - (bulletSpawn.right * offsetStart) - (bulletSpawn.up * offsetStart), bulletSpawn.forward - (bulletSpawn.right * rayDetectorMeeleSpace) - (bulletSpawn.up * rayDetectorMeeleSpace));
			ray9 = new Ray(bulletSpawn.position - (bulletSpawn.up * offsetStart), bulletSpawn.forward - (bulletSpawn.up * rayDetectorMeeleSpace));

			Debug.DrawRay(ray1.origin, ray1.direction, Color.cyan);
			Debug.DrawRay(ray2.origin, ray2.direction, Color.cyan);
			Debug.DrawRay(ray3.origin, ray3.direction, Color.cyan);
			Debug.DrawRay(ray4.origin, ray4.direction, Color.red);
			Debug.DrawRay(ray5.origin, ray5.direction, Color.red);
			Debug.DrawRay(ray6.origin, ray6.direction, Color.red);
			Debug.DrawRay(ray7.origin, ray7.direction, Color.yellow);
			Debug.DrawRay(ray8.origin, ray8.direction, Color.yellow);
			Debug.DrawRay(ray9.origin, ray9.direction, Color.yellow);

			if (GetComponent<GunInventory>().currentGun)
			{
				if (GetComponent<GunInventory>().currentGun.GetComponent<GunScript>().meeleAttack == false)
				{
					been_to_meele_anim = false;
				}
				if (GetComponent<GunInventory>().currentGun.GetComponent<GunScript>().meeleAttack == true && been_to_meele_anim == false)
				{
					been_to_meele_anim = true;
					//	if (isRunning == false) {
					StartCoroutine("MeeleAttackWeaponHit");
					//	}
				}
			}
		}
	}

	//ダメージを受けたときの処理
    public void Damage()
    {
        damageVoice.Play(); //ダメージボイス再生
    }

    public GameObject button;
    public void Die()
    {
        //プレイヤーが死亡したときの処理
        GunInventory.gunInventoryInstance.DeadMethod();
        MouseLookScript mouseLookScript = GetComponent<MouseLookScript>();
        GunInventory gunInventory = GetComponent<GunInventory>();
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        PlayerMovementScript playerMovementScript = GetComponent<PlayerMovementScript>();

        mouseLookScript.enabled = false; //視点移動無効
        gunInventory.enabled = false; //銃未所持
        rigidbody.freezeRotation = false; //物理的回転を有効
        playerMovementScript.enabled = false; //プレイヤーを動けなくする

        StartCoroutine(FadeInGameOverText());　//ゲームオーバーテキストを表示
       
        Cursor.lockState = CursorLockMode.Confined; //マウス表示
    }

	[SerializeField] private TextMeshProUGUI gameOverText; //ゲームオーバーテキスト
    [SerializeField]private float fadeDuration = 2f; //テキストがフェードインにかける時間 
    private Color originalColor; //テキストカラー

	//ゲームオーバーのテキストを徐々に表示する
    private IEnumerator FadeInGameOverText()
    {
        yield return new WaitForSeconds(2f);

        float elapsed = 0f;

		//fadeDuration秒かけてテキストを表示
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            Color c = originalColor;
            c.a = alpha;
            gameOverText.color = c;
            yield return null;
        }

        // 最終的に完全に表示
        gameOverText.color = originalColor;
        button.SetActive(true);
    }

	[Tooltip("歩行時の足音")]
	public AudioSource _walkSound;
	[Tooltip("走行時の足音")]
	public AudioSource _runSound;
	[Tooltip("キーアイテム取得時のサウンド")]
	public AudioSource pickUpSound;
	[Tooltip("ダメージボイス")]
    public AudioSource damageVoice;
}

