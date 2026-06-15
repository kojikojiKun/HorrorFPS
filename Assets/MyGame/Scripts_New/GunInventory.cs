using System.Collections.Generic;
using UnityEngine;

public class GunInventory
{
    private Dictionary<GunData, GameObject> instanceDict = new();
    private List<GunData> primariesData = new();
    private int primariesIndex;
    private GunData secondaryData;
    private bool isSetData = false;
    private GunData currentGunData;
    public GunData CurrentGunData => currentGunData;

    public void InstantiateGuns(GunData[] gunsData, Transform spawnGunPos, Transform gunsPivot)
    {
        foreach (var data in gunsData)
        {
            //データの重複をチェック.
            if (instanceDict.ContainsKey(data))
            {
                Debug.LogError($"重複キー: {data.name}");
                continue;
            }

            //オブジェクト生成.
            GameObject gun = Object.Instantiate(
                data.GunPrefab,
                spawnGunPos.position,
                Quaternion.identity,
                gunsPivot
                );

            //非表示にする.
            gun.SetActive(false);

            //Dictionary(Key:GunsData,Value:GameObject)に追加.
            instanceDict.Add(data, gun);

            //武器をプライマリー武器とセカンダリー武器に分類.
            if (data.EquipType == EquipType.Secondary)
            {
                secondaryData = data;
            }
            else
            {
                primariesData.Add(data);
            }
        }

        isSetData = true;
    }

    public void ChangeGun(EquipType type)
    {
        if (!isSetData)
        {
            Debug.LogError("Require Data is not Set");
            return;
        }

        if (currentGunData == null)
        {
            EquipGun(secondaryData);
        }

        //要求されたEquipTypeによって取得するデータを変える.
        GunData gunData = type switch
        {
            EquipType.Primary => GetPrimaryGun(),
            EquipType.Secondary => GetSecondaryGun(),
            _ => null
        };

        if (gunData == null)
        {
            Debug.LogError("GunData is null");
            return;
        }

        EquipGun(gunData);
    }

    private GunData GetPrimaryGun()
    {
        //現在プライマリー武器を装備中のとき.
        if (currentGunData.EquipType == EquipType.Primary)
        {
            //primariesDataの要素を循環選択.
            primariesIndex = (primariesIndex + 1) % primariesData.Count;
        }

        return primariesData[primariesIndex];
    }

    private GunData GetSecondaryGun()
    {
        //現在プライマリー武器を装備中のとき.
        if (currentGunData.EquipType == EquipType.Primary)
        {
            return secondaryData;
        }

        return primariesData[primariesIndex];
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
        else
        {
            Debug.LogError($"Gun not found{data.name}");
        }
    }
}