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
    [Tooltip("装備タイプ(プライマリ、セカンダリ)")] public EquipType EquipType;
    [Tooltip("リロードをせずに発射できる弾丸の数")] public int MagazineSize;
    [Tooltip("1分間で発射できる弾丸の数")] public float FireRate;
    [Tooltip("弾丸が敵に与えるダメージ(散弾銃は1ペレットごとのダメージ)")] public int BulletDamage;
    [Tooltip("水平方向の反動の大きさ")] public float Recoil_X;
    [Tooltip("垂直方向の反動の大きさ")] public float Recoil_Y;
}
