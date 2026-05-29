using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    public event Action<Vector2> OnMove;
    public event Action<Vector2> OnLook;
    public event Action OnChangeGun;
    public event Action<bool> OnRun;
    public event Action<bool> OnFire;
    public event Action<bool> OnAiming;
    public event Action OnOpenMenu;

    public void Move(InputAction.CallbackContext context)
    {
        //移動入力があるとき.
        if (context.performed)
        {
            //入力された値を取得して、OnMoveイベントを呼び出す.
            Vector2 input = context.ReadValue<Vector2>();
            OnMove?.Invoke(input);
        }
    }

    public void Look(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //入力された値を取得して、OnLookイベントを呼び出す.
            Vector2 input = context.ReadValue<Vector2>();
            OnLook?.Invoke(input);
        }
    }

    public void ChangeGun(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnChangeGun?.Invoke();
    }

    public void Run(InputAction.CallbackContext context)
    {
        //走る入力がある間、OnRunイベントを呼び出す.
        if (context.performed)
            OnRun?.Invoke(true);
        else if (context.canceled)
            OnRun?.Invoke(false);
    }

    public void Fire(InputAction.CallbackContext context)
    {
        //射撃入力がある間、OnFireイベントを呼び出す.
        if (context.performed)
            OnFire?.Invoke(true);
        else if (context.canceled)
            OnFire?.Invoke(false);
    }

    public void Aiming(InputAction.CallbackContext context)
    {
        //エイム入力がある間、OnAimingイベントを呼び出す.
        if (context.performed)
            OnAiming?.Invoke(true);
        else if (context.canceled)
            OnAiming?.Invoke(false);
    }

    public void OpenMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnOpenMenu?.Invoke();
    }
}
