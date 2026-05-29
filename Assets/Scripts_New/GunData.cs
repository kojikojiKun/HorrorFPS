using UnityEngine;

public enum FireType
{
    Automatic, //フルオート.
    NonAutomatic, //単発.
    ShotGun //散弾.
}

[CreateAssetMenu(fileName ="Data",menuName ="ScriptableObject/Gun")]
public class GunData : ScriptableObject
{
    public GameObject GunPrefab;
    public FireType Type;
    public int MagazineSize;
    public float FireRate;
    public int BulletDamage;
    public float Recoil_X;
    public float Recoil_Y;
}
