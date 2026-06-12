using UnityEngine;

public class PlayerMovement
{
    private CharacterController characterController;
    private Transform cameraPos;
    private PlayerStatus status;
    private const float GRAVITY = -9.8f;
    private float velocity = 0f;
    private float moveSpeed;

    public PlayerMovement(CharacterController cc,Transform camPos,PlayerStatus ps)
    {
        characterController = cc;
        cameraPos = camPos;
        status = ps;

        SetStatus();
    }

    private void SetStatus()
    {
        moveSpeed = status.MoveSpeed;
    }

    private Vector3 FreeFall()
    {
        //接地中に地面に押し付ける.
        if (characterController.isGrounded && velocity < 0)
            velocity = -2f;

        //y軸下向きに力を加える.
        velocity += GRAVITY * Time.deltaTime;
        return Vector3.up * velocity * Time.deltaTime;
    }

    public void Move(Vector2 input)
    {
        //移動入力,カメラの向きからベクトルを計算.
        Vector3 dir = (cameraPos.forward * input.y) + (cameraPos.right * input.x);

        //ベクトルを正規化.
        if (dir.sqrMagnitude > 1)
            dir.Normalize();

        //移動方向を決定.
        Vector3 horizontal = dir * moveSpeed * Time.deltaTime;

        //実際に移動させる.
        characterController.Move(horizontal+FreeFall());
    }
}
