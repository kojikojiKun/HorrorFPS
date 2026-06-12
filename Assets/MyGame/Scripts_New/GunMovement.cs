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
    private GunData selectedGunData;
    private GunData currentGunData;

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
        if (currentGunData == null)
        {
            Debug.LogError("currentGunData is null");
            return;
        }

        //要求された銃タイプで分岐.
        switch (type)
        {
            case EquipType.Primary:

                //現在装備中の銃タイプで分岐.
                switch (currentGunData.EquipType)
                {
                    case EquipType.Primary:
                        //プライマリー武器を循環選択.
                        primariesIndex++;
                        if (primariesIndex >= primariesData.Length)
                            primariesIndex = 0;
                        break;
                    case EquipType.Secondary:
                        break;
                }

                selectedGunData = primariesData[primariesIndex];
                break;

            case EquipType.Secondary:

                //現在装備中の銃タイプで分岐.
                switch (currentGunData.EquipType)
                {
                    //最後にプライマリー武器を装備していたとき.
                    case EquipType.Primary:
                        selectedGunData = secondaryData;
                        break;
                    //最後にセカンダリー武器を装備していたとき.
                    case EquipType.Secondary:
                        selectedGunData = primariesData[primariesIndex];
                        break;
                }
                break;

            default:
                Debug.LogError("EquipType Error");
                break;
        }

        //銃を装備.
        EquipGun(selectedGunData);
    }

    public void EquipGun(GunData data)
    {
        if (data == null)
        {
            Debug.LogError("data is null");
            return;
        }

        //すべてのオブジェクトを非表示.
        foreach (var value in instanceDict.Values)
        {
            value.SetActive(false);
        }

        //DictionaryからKey:GunDataに対応したGameObjectを取り出す.
        if (instanceDict.TryGetValue(data, out GameObject selectedGun))
        {
            //選択された銃を表示.
            selectedGun.SetActive(true);
            currentGunData = data;
        }
    }

    public void FollowCamera()
    {
        //カメラの向きに銃を追従させる.
        gunPivot.forward = camPos.forward;
    }
}
