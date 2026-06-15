using UnityEngine;

public class GunShooter
{
    private GunData currentEquipGunData;
    private float nextFireTime;

    public void SetCurrentEquipGunData(GunData data)
    {
        //装備中の銃のデータをセットする.
        currentEquipGunData = data;
    }

    //射撃可能かどうかを返す.
    private bool CanFire(bool isPushing) {
        return currentEquipGunData != null ||
            currentEquipGunData.FireRate != 0 ||
            isPushing ||
            Time.time < nextFireTime;
    }

    //発射処理の更新.
    public void UpDateFire(bool isPushing)
    {
        if (!CanFire(isPushing))
            return;

        Fire();

        nextFireTime = Time.time + (60f / currentEquipGunData.FireRate);
    }

    private void Fire()
    {
        Debug.Log("shot");
    }
}
