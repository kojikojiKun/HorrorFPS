using UnityEngine;

public enum FireType
{
    Automatic, //フルオート.
    NonAutomatic, //単発.
    ShotGun //散弾.
}

public enum EquipType
{
    Primary,
    Secondary
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/Gun")]
public class GunData : ScriptableObject
{
    public GameObject GunPrefab;
    public Texture CrossHair;
    public FireType FireType;
    public EquipType EquipType;
    public int Index;
    public int MagazineSize;
    public float FireRate;
    public int BulletDamage;
    public float Recoil_X;
    public float Recoil_Y;
}
