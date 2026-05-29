using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(ControllerReader))]
[RequireComponent(typeof(InputReader))]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    private ControllerReader controllerReader;
    private InputReader inputReader;
    private static Gamepad pad;

    public InputReader InputReader => inputReader;
    public static Gamepad CurrentPad => pad;

    private void Awake()
    {
        Instantiation();

        controllerReader = GetComponent<ControllerReader>();
        inputReader = GetComponent<InputReader>();
    }

    private void Instantiation()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        //インスタンス化し、シーンをまたいでも破壊しない.
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        pad = controllerReader.CurrentGamePad();
    }  
}
