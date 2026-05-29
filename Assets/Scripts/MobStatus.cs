using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MobStatus : MonoBehaviour
{
    /// <summary>
    /// 状態の定義
    /// </summary>
    public enum StateEnum
    {
        Normal, // 通常
        Attack, // 攻撃中
        Die // 死亡
    }

    /// <summary>
    /// 移動可能かどうか
    /// </summary>
    public bool IsMovable => StateEnum.Normal == _state;

    /// <summary>
    /// 攻撃可能かどうか
    /// </summary>
    public bool IsAttackable => StateEnum.Normal == _state;

    /// <summary>
    /// ライフ最大値を返します
    /// </summary>
    public float LifeMax => lifeMax;

    /// <summary>
    /// ライフの値を返します
    /// </summary>
    public float Life => _life;

    [SerializeField] private float lifeMax = 10; // ライフ最大値
    private NavMeshAgent agent;
    protected Animator _animator;
    public StateEnum _state = StateEnum.Normal; // Mob状態
    private float _life; // 現在のライフ値（ヒットポイント）
    private float speed;

    public AudioSource dieSound;
    public AudioSource attakSound;
    private float startTime = 1f;
    private float duration = 0.8f;

    [SerializeField]
    private ChasePlayer chasePlayer;

    protected virtual void Start()
    {
        _life = lifeMax; // 初期状態はライフ満タン
        _animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        _state = StateEnum.Normal;
        speed = agent.speed;
    }

    private void Update()
    {
        if (_state != StateEnum.Die && chasePlayer.findPlayer==false)
        {
            Patrol();
        }
    }

    public float patrolRadius = 10f;       // 徘徊範囲
    public float waitTime = 3f;            // 目的地に到達後の待機時間
    private float waitTimer = 0f;
    private bool isWaiting = false;
    public AudioSource search1;
    public AudioSource search2;

    void Patrol()
    {
        if (isWaiting == true)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                MoveToRandomPoint();
            }

            return;
        }

        // 到達判定：agent.remainingDistanceが小さく、停止している
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            float distance=Vector3.Distance(chasePlayer.goal.transform.position,transform.position);
            if (distance < 30)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    isWaiting = true;

                    int random = Random.Range(0, 2);
                    if (random > 0)
                    {

                        search1.Play(); //停止中に敵の声を再生
                    }
                    else
                    {
                        search2.Play();//停止中に敵の声を再生
                    }

                    waitTimer = 0f;
                }
            }
        }
    }

    //ランダムで移動する
    void MoveToRandomPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    /// <summary>
    /// キャラが倒れた時の処理を記述します。
    /// </summary>
    /// 


    protected virtual void OnDie()
    {
        agent.speed = 0;
        dieSound.Play();
        Destroy(gameObject, 3f);    //3秒後にオブジェクトを破壊
    }

    /// <summary>
    /// 指定値のダメージを受けます。
    /// </summary>
    /// <param name="damage"></param>
    /// 

    IEnumerator speedControl()
    {
        yield return new WaitForSeconds(0.5f);
        agent.speed = speed; //スピードを元に戻す
    }

    private bool isDamaged = false;

    public void Damage(int damage)
    {

        Debug.LogWarning("ダメージを受けた");

        if (_state == StateEnum.Die) return;

        _life -= damage;

        //体力が半分になったらアニメーションを再生
        if (_life <= lifeMax / 2 && isDamaged == false)
        {
            _animator.SetTrigger("damage");
            agent.speed = 0;
            StartCoroutine(speedControl());
            isDamaged = true;
        }

        if (_life > 0) return;

        if (_life <= 0)
        {
            _state = StateEnum.Die;
            _animator.SetTrigger("die");

            OnDie();
        }
    }

    /// <summary>
    /// 可能であれば攻撃中の状態に遷移します。
    /// </summary>
    public void GoToAttackStateIfPossible()
    {
        if (!IsAttackable) return;

        _state = StateEnum.Attack;
        _animator.SetTrigger("attack");
        StartCoroutine(PlayPartOfAudio());
    }

    private IEnumerator PlayPartOfAudio()
    {
        attakSound.time = startTime;
        attakSound.Play();
        yield return new WaitForSeconds(duration);
        attakSound.Stop();
    }

    /// <summary>
    /// 可能であればNormalの状態に遷移します。
    /// </summary>
    public void GoToNormalStateIfPossible()
    {
        if (_state == StateEnum.Die) return;

        _state = StateEnum.Normal;
    }
}
