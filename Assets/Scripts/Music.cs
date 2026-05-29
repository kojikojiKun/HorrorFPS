using UnityEngine;

public class Music : MonoBehaviour
{
    [SerializeField]
    private AudioSource music;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        music.Play();  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
