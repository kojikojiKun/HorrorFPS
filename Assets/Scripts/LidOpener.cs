using UnityEngine;

public class LidOpener : MonoBehaviour
{
    public Transform lid;
    public Vector3 openOffset = new Vector3(0, 0.2f, 0); // äJÇ¢ÇΩÇ∆Ç´ÇÃà íuÇÃç∑ï™
    public float speed = 5f;

    private bool isOpen = false;
    private Vector3 closedPosition;
    private Vector3 openPosition;

    void Start()
    {
        closedPosition = lid.localPosition;
        openPosition = closedPosition + openOffset;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
        }

        Vector3 targetPosition = isOpen ? openPosition : closedPosition;
        lid.localPosition = Vector3.Lerp(lid.localPosition, targetPosition, Time.deltaTime * speed);
    }
}
      