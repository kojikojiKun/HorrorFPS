using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPos;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.1f;  // —h‚ê‚Ì‹­‚³
    private float dampingSpeed = 1.0f;    // —h‚ê‚ÌŒ¸Š‘¬“x

    void OnEnable()
    {
        originalPos = transform.localPosition;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = originalPos;
        }
    }

    public void Shake(float duration, float magnitude)
    {
        //duration•b,magnitude‚Ì‹­‚³‚Å—h‚ç‚·
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }
}
