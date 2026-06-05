using UnityEngine;
using System.Collections.Generic;

public class GunMovement
{
    private Transform camPos;
    private Transform gunPivot;
    private Dictionary<GunType, GameObject> instanceDict = new();

    public GunMovement(Camera cam,Transform pivot)
    {
        camPos = cam.transform;
        gunPivot = pivot;
    }

    public void GetGunInstancesDictionary(Dictionary<GunType, GameObject> dict)
    {
        //シーン上の銃オブジェクトインスタンスを登録したDictionaryを受け取って代入.
        instanceDict = dict;
    }

    public void FollowCamera()
    {
        //カメラの向きに銃を追従させる.
        gunPivot.forward = camPos.forward;
    }

    public void Equip(GunType type)
    {
        //指定されたGunTypeのオブジェクトを取り出す.
        if (instanceDict.TryGetValue(type, out GameObject gun))
        {
            //銃オブジェクトを表示.
            gun.SetActive(true);
        }
    }
}
