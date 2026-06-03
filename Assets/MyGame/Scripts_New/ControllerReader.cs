using UnityEngine.InputSystem;
using UnityEngine;

public class ControllerReader : MonoBehaviour
{
    private static Gamepad currentPad;

    public void CheckCurrentController()
    {
        foreach (var pad in Gamepad.all)
        {
            //最後に入力があったコントローラーを使用コントローラーとして登録.
            if (pad.wasUpdatedThisFrame)
            {
                currentPad = pad;
            }
        }
    }

    public Gamepad CurrentGamePad()
    {
        return currentPad;
    }
}
