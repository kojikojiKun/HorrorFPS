using UnityEngine;
using System.Collections;


public class MouseLookScript : MonoBehaviour
{

	[HideInInspector]
	public Transform myCamera;

	void Awake()
	{
		Cursor.lockState = CursorLockMode.Locked;
		myCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
		Application.targetFrameRate = 60;
	}

	void Update()
	{

		MouseInputMovement();

		if (Input.GetKeyDown(KeyCode.L))
		{
			Cursor.lockState = CursorLockMode.Locked;

		}
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

		if (GetComponent<PlayerMovementScript>().currentSpeed > 1)
			HeadMovement();

	}

	[HideInInspector] public float timer;
	[HideInInspector] public int int_timer;
	[HideInInspector] public float zRotation;
	[HideInInspector] public float wantedZ;
	[HideInInspector] public float timeSpeed = 2;

	[HideInInspector] public float timerToRotateZ;

	void HeadMovement()
	{
		timer += timeSpeed * Time.deltaTime;
		int_timer = Mathf.RoundToInt(timer);
		if (int_timer % 2 == 0)
		{
			wantedZ = -1;
		}
		else
		{
			wantedZ = 1;
		}

		zRotation = Mathf.Lerp(zRotation, wantedZ, Time.deltaTime * timerToRotateZ);
	}
	[Tooltip("Current mouse sensivity, changes in the weapon properties")]
	public float mouseSensitvity = 0;
	[HideInInspector]
	public float mouseSensitvity_notAiming = 300;
	[HideInInspector]
	public float mouseSensitvity_aiming = 50;

	void FixedUpdate()
	{
		if (Input.GetAxis("Fire2") != 0)
		{
			mouseSensitvity = mouseSensitvity_aiming;
		}
		else if (GetComponent<PlayerMovementScript>().maxSpeed > 5)
		{
			mouseSensitvity = mouseSensitvity_notAiming;
		}
		else
		{
			mouseSensitvity = mouseSensitvity_notAiming;
		}
		ApplyingStuff();
	}


	private float rotationYVelocity, cameraXVelocity;
	public float yRotationSpeed, xCameraSpeed;

	[HideInInspector]
	public float wantedYRotation;
	[HideInInspector]
	public float currentYRotation;

	[HideInInspector]
	public float wantedCameraXRotation;
	[HideInInspector]
	public float currentCameraXRotation;

	public float topAngleView = 60;
	public float bottomAngleView = -45;

	void MouseInputMovement()
	{

		wantedYRotation += Input.GetAxis("Mouse X") * mouseSensitvity;

		wantedCameraXRotation -= Input.GetAxis("Mouse Y") * mouseSensitvity;

		wantedCameraXRotation = Mathf.Clamp(wantedCameraXRotation, bottomAngleView, topAngleView);

	}

	void ApplyingStuff()
	{

		currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotationYVelocity, yRotationSpeed);
		currentCameraXRotation = Mathf.SmoothDamp(currentCameraXRotation, wantedCameraXRotation, ref cameraXVelocity, xCameraSpeed);

		WeaponRotation();

		transform.rotation = Quaternion.Euler(0, currentYRotation, 0);
		myCamera.localRotation = Quaternion.Euler(currentCameraXRotation, 0, zRotation);

	}

	[Tooltip("Current weapon that player carries.")]
	[HideInInspector]
	public GameObject weapon;
	private GunScript gun;

	void WeaponRotation()
	{
		if (!weapon)
		{
			weapon = GameObject.FindGameObjectWithTag("Weapon");
			if (weapon)
			{
				if (weapon.GetComponent<GunScript>())
				{
					gun = GameObject.FindGameObjectWithTag("Weapon").GetComponent<GunScript>();
					if (gun == null)
						Debug.LogError("gun is null");

				}
			}
		}

	}
	float deltaTime = 0.0f;
}
