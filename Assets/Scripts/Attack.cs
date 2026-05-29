using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MobStatus))]
public class Attack : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 3f; // 攻撃後のクールダウン（秒）
    [SerializeField] private Collider attackCollider;

    private MobStatus _status;
    private NavMeshAgent agent;

    private void Start()
    {
        _status = GetComponent<MobStatus>();
        agent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// 攻撃可能な状態であれば攻撃を行います。
    /// </summary>
    public void AttackIfPossible()
    {
        if (!_status.IsAttackable) return; // ステータスと衝突したオブジェクトで攻撃可否を判断

        _status.GoToAttackStateIfPossible();
    }

    /// <summary>
    /// 攻撃対象が攻撃範囲に入った時に呼ばれます。
    /// </summary>
    /// <param name="collider"></param>
    public void OnAttackRangeEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        AttackIfPossible();
    }

    /// <summary>
    /// 攻撃の開始時に呼ばれます。
    /// </summary>
    public void OnAttackStart()
    {
        attackCollider.enabled = true;
        agent.speed *= 0.2f;
    }

    /// <summary>
    /// attackColliderが攻撃対象にHitした時に呼ばれます。
    /// </summary>
    /// <param name="collider"></param>
    /// 
    public int Damage;
    public void OnHitAttack(Collider collider)
    {
        var targetMob = collider.GetComponent<TakeDamagePlayer>();
        if (null == targetMob) return;

        // プレイヤーにダメージを与える
        targetMob.TakeDamage(Damage);
    }

    /// <summary>
    /// 攻撃の終了時に呼ばれます。
    /// </summary>
    public void OnAttackFinished()
    {
        attackCollider.enabled = false;
        
        StartCoroutine(CooldownCoroutine());
        StartCoroutine(SpeedControl());
        
    }

    private IEnumerator SpeedControl()
    {
        yield return new WaitForSeconds(1f);
        agent.speed /= 0.2f; //攻撃が終わると元のスピードに戻す
    }

    private IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(attackCooldown);
        _status.GoToNormalStateIfPossible();
    }
}
