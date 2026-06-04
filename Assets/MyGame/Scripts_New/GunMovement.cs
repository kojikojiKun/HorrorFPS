using UnityEngine;
using System.Collections.Generic;

public class GunMovement
{
    private GameObject gunPrefab;
    private Transform camPos;
    private Dictionary<GunType, GameObject> instanceDict = new();

    public void GetGunInstancesDictionary(Dictionary<GunType, GameObject>dict)
    {
        instanceDict = dict;
    }

    //ƒJƒƒ‰‚ÌŒü‚«‚Ée‚ğ’Ç]‚³‚¹‚é.
    public void FollowCamera()
    {
        gunPrefab.transform.forward = camPos.forward;
    }

    //e‚ğ‘•”õ.
    public void Equip(GunType type)
    {
        if(instanceDict.TryGetValue(type,out GameObject gun))
        {
            gun.SetActive(true);
        }
    }
}
