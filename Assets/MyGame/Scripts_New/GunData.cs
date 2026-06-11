using UnityEngine;

public enum FireType
{
    Automatic, //フルオート.
    SemiAuto, //単発.
}

public enum EquipType
{
    Primary,
    Secondary
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/Gun")]
public class GunData : ScriptableObject
{
    [Tooltip("銃のプレファブ")] public GameObject GunPrefab;
    [Tooltip("クロスヘアのテクスチャ")] public Texture CrossHair;
    [Tooltip("射撃タイプ（フルオート、セミオート）")] public FireType FireType;
    public EquipType EquipType;
    public int Index;
    public int MagazineSize;
    public float FireRate;
    public int BulletDamage;
    public float Recoil_X;
    public float Recoil_Y;
}
