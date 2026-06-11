using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GunMovement
{
    private Transform camPos;
    private Transform gunPivot;
    private Dictionary<GunData, GameObject> instanceDict = new();
    private GunData[] primariesData;
    private GunData secondaryData;
    private int primariesIndex;
    private GunData lastEquipGunData;

    public GunMovement(Camera cam, Transform pivot)
    {
        camPos = cam.transform;
        gunPivot = pivot;
    }

    public void GetGunsData(
        Dictionary<GunData, GameObject> dict,
        GunData[] primaries,
        GunData secondary
        )
    {
        //シーン上の銃オブジェクトインスタンスを登録したDictionaryを代入.
        instanceDict = dict;

        //プライマリー武器データが格納された配列を代入.
        primariesData = primaries;

        //セカンダリー武器データを代入.
        secondaryData = secondary;
    }

    public void ChangeGun(EquipType type)
    {
        switch (type)
        {
            case EquipType.Primary:
                //最後にプライマリー武器を装備していたとき.
                if (lastEquipGunData.EquipType == EquipType.Primary)
                {
                    //プライマリー武器を循環選択.
                    primariesIndex++;
                    if (primariesIndex >= primariesData.Length)
                        primariesIndex = 0;

                    EquipGun(primariesData[primariesIndex]);
                }
                break;
            case EquipType.Secondary:
                switch (lastEquipGunData.EquipType)
                {
                    //最後にプライマリー武器を装備していたとき.
                    case EquipType.Primary:
                        EquipGun(lastEquipGunData);
                        break;
                    //最後にセカンダリー武器を装備していたとき.
                    case EquipType.Secondary:
                        break;
                }
                break;
            default:
                Debug.LogError("EquipType Error");
                break;
        }
    }

    private void EquipGun(GunData data)
    {
        //すべてのオブジェクトを非表示.
        foreach (var value in instanceDict.Values)
        {
            value.SetActive(false);
        }

        /*DictionaryからKey:GunDataに対応したGameObjectを取り出す.
        if (instanceDict.TryGetValue(data, out GameObject selectedGun))
        {
            //選択された銃を表示.
            selectedGun.SetActive(true);
        }*/
    }

    public void FollowCamera()
    {
        //カメラの向きに銃を追従させる.
        gunPivot.forward = camPos.forward;
    }
}
