using UnityEngine;
using System.Collections.Generic;

public class GunMovement
{
    private Transform camPos;
    private Transform gunPivot;
    private Dictionary<int, GameObject> instanceDict = new();

    public GunMovement(Camera cam, Transform pivot)
    {
        camPos = cam.transform;
        gunPivot = pivot;
    }

    public void GetGunInstancesDictionary(Dictionary<int, GameObject> dict)
    {
        //シーン上の銃オブジェクトインスタンスを登録したDictionaryを受け取って代入.
        instanceDict = dict;
    }

    public void FollowCamera()
    {
        //カメラの向きに銃を追従させる.
        gunPivot.forward = camPos.forward;
    }

    public void EquipGun()
    {
        //すべてのオブジェクトを非表示.
        foreach (var value in instanceDict.Values)
        {
            value.SetActive(false);
        }
    }
}
