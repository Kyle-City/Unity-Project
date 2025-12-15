using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 3f; // 敌人移动速度
    public float chaseRange = 10f; // 追击范围
    public float attackRange = 1f; // 攻击范围
    public float attackCooldown = 3f; // 攻击冷却时间
    public Slider enemySlider; // 敌人血条
    public Slider playerSilder; // 主角血条
    public float damage = 0.1f; // 受到的伤害

    private Animator animator; // 动画控制器，用于控制敌人的动画
    private Transform target; // 主角的位置
    private float lastAttackTime = 0f; // 记录上次攻击的时间
    private NavMeshAgent agent; // 导航代理组件
    private float distanceToTarget; // 敌人与主角的距离

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform; // 获取主角位置
        animator = GetComponent<Animator>(); // 获取动画控制器
        agent = GetComponent<NavMeshAgent>(); // 获取导航代理组件
        agent.speed = moveSpeed; // 设置导航代理的移动速度
    }

    void Update()
    {
        distanceToTarget = Vector3.Distance(transform.position, target.position); // 计算敌人与主角的距离

        if (distanceToTarget <= chaseRange && enemySlider.value > 0.05) // 如果主角在追击范围内
        {
            agent.SetDestination(target.position); // 设置导航代理的目标为主角位置
            animator.SetBool("Move", true); // 播放移动动画
            agent.isStopped = false; // 恢复导航代理的移动

            if (distanceToTarget <= attackRange) // 如果敌人在攻击范围内
            {
                agent.isStopped = true; // 停止导航代理的移动
                if (Time.time >= lastAttackTime + attackCooldown) // 如果攻击冷却时间已过
                {
                    lastAttackTime = Time.time; // 更新上次攻击时间
                    StartCoroutine(DelayedAttack()); // 延迟执行攻击逻辑
                }
            }          
        }
        else
        {
            animator.SetBool("Move", false); // 如果主角不在追击范围内，停止移动动画
            agent.isStopped = true; // 停止导航代理的移动
        }
    }

    private IEnumerator DelayedAttack()
    {
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f); // 等待0.5秒以模拟攻击延迟
        if (distanceToTarget <= attackRange)
        {
            playerSilder.value -= 0.1f; // 减少敌人的血量 
        }   
    }

    private void OnTriggerEnter(Collider other)
    {
        // 如果敌人与子弹发生碰撞
        if (other.gameObject.tag == "Bullet")
        {
            Destroy(other.gameObject); // 销毁子弹
            enemySlider.value -= damage; // 减少敌人的生命值

            // 如果敌人生命值小于等于0，销毁敌人对象
            if (enemySlider.value <= 0.01f)
            {
                Destroy(gameObject); // 销毁敌人
            }
        }
    }
}

