using UnityEngine;

public class OptionsMenuController : MonoBehaviour
{
    public GameObject optionsMenu;  // オプションUIをここにアサイン
    public bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.Confined;
            ToggleOptionsMenu();

            
        }
        
    }

    void ToggleOptionsMenu()
    {
        isPaused = !isPaused;
        optionsMenu.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f; // ゲーム停止
        }
        else
        {
            Time.timeScale = 1f; // ゲーム再開
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
