using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip startSound;

    public void PlayStartSound()
    {
        audioSource.PlayOneShot(startSound);
    }
}