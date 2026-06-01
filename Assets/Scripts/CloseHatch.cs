using UnityEngine;

public class CloseHatch : MonoBehaviour
{
    //public SceneController sceneController;

   // private GameObject hatchJoint;
    private bool isPlayerEnter=false;

    public float rotationSpeed = 30f;
    public float maxRotation = 55f;
    private float rotatedAngle = 0;

    private void Update()
    {
        if (isPlayerEnter == true)
        {
            //âÒì]äpìxêßå¿
            if (rotatedAngle < maxRotation)
            {
                float angleThisFrame = rotationSpeed * Time.deltaTime;

                // écÇËÇÃâÒì]äpìxÇåvéZ
                float remaining = maxRotation - rotatedAngle;

                // écÇËÇÊÇËëÂÇ´Ç≠âÒì]ÇµÇ»Ç¢ÇÊÇ§í≤êÆ
                float angle = Mathf.Min(angleThisFrame, remaining);

                Vector3 pivot = gameObject.transform.position;
                Vector3 axis = Vector3.forward;

                transform.RotateAround(pivot, axis, angle);

                rotatedAngle += angle;
            }
            else
            {
                Invoke("LoadScene", 1.5f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           // hatchJoint = GameObject.FindGameObjectWithTag("HatchJoint");
            isPlayerEnter = true;
        }
    }

    void LoadScene()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }
}
