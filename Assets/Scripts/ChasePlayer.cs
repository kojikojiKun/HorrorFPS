using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChasePlayer : MonoBehaviour
{
    public GameObject goal; //これにプレイヤーを格納
    public NavMeshAgent agent; //①敵が自動で動くために必要
    public float Distance; //②プレイヤーと敵の距離を格納する変数(distane=距離)
    public float chaseDistance; //プレイヤーを追いかける距離
    private float DefaultChaseDistance;
    public Animator animator;

    public AudioSource findPlayerSound;
    public MobStatus mobStatus;
   // public EnemyPatrol enemyPatrol;

    private float defaultSpeed;
    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();　//①
        animator = GetComponent<Animator>();
        goal = GameObject.Find("Player");

        DefaultChaseDistance = chaseDistance;//最初の追跡する距離を保存

        if (mobStatus == null)
        {
            mobStatus.GetComponent<MobStatus>();
        }

        defaultSpeed = agent.speed;
    }

    public bool findPlayer = false;
    public float stopDistance;

    // Update is called once per frame
    void Update()
    { 
        //②二者間の距離を計算してfloat　一定値いかになれば追跡する
        Distance = Vector3.Distance(goal.transform.position,transform.position);

        if (Distance < stopDistance)
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
            if (Distance < chaseDistance && mobStatus._state != MobStatus.StateEnum.Die)
            {
                if (findPlayer == false )
                {
                    findPlayerSound.Play();
                    Debug.Log(findPlayer);
                    findPlayer = true;
                }
                agent.SetDestination(goal.transform.position);
                agent.speed = defaultSpeed;//スピードを元に戻す
            }
            else
            {
                // 距離が離れたらフラグを戻す
                findPlayer = false;
                //enemyPatrol.enabled = true;
            }
        }

        //キャラクターの移動スピード
        float speed = agent.velocity.magnitude;

        animator.SetFloat("MoveSpeed",speed);
    }

    //打たれたら索敵範囲拡大
    public void ShotPlayer()
    {
        chaseDistance = 1000;
        findPlayer=true;
        StartCoroutine(FindPlayer());
    }

    private IEnumerator FindPlayer()
    {
        yield return new WaitForSeconds(1f);
        chaseDistance = DefaultChaseDistance;
    }
}
