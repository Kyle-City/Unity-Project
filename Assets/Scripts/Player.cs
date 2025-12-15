using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f; // 玩家移动速度
    public float jumpForce = 10f; // 跳跃力量
    public GameObject bulletPrefab; // 子弹预制体
    public Transform firePoint; // 发射点
    public float bulletSpeed = 10f; // 子弹飞行速度
    public float rotationSpeed = 5f; // 主角旋转速度
    public float attackCooldown = 0.5f; // 攻击冷却时间
    public GameObject failPanel; // 失败面板
    public GameObject winPanel; // 胜利面板
    public Text remainText; // 显示剩余敌人敌人数量的文本
    public Transform checkGround; // 检测是否在地面上的位置
    public Slider slider; // 玩家生命值滑动条

    private bool isGrounded = false; // 是否在地面上
    private float attackTimer = 0f; // 攻击冷却计时器
    private Animator animator; // 动画控制器
    private bool isEnd = false; // 游戏结束标志

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 锁定鼠标光标
        animator = GetComponent<Animator>(); // 获取Animator组件
    }

    void Update()
    {
        if (isEnd) return; // 如果游戏结束，不再更新

        // 获取玩家的水平和垂直输入
        float moveInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveInput, 0, verticalInput);

        // 如果有移动输入，则播放移动动画并移动玩家
        if (movement != Vector3.zero)
        {
            animator.SetBool("Move", true);
            transform.Translate(movement * moveSpeed * Time.deltaTime); // 移动玩家
        }
        else
        {
            animator.SetBool("Move", false); // 无输入时停止移动动画
        }

        // 旋转玩家视角
        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up * mouseX * rotationSpeed); // 根据鼠标水平移动旋转玩家

        // 检测是否在地面上
        isGrounded = Physics.CheckSphere(checkGround.position, 0.5f, LayerMask.GetMask("Ground"));

        // 按下空格键且角色在地面上时跳跃
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            GetComponent<Rigidbody>().AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse); // 向上施加力实现跳跃
        }

        // 攻击冷却逻辑
        attackTimer -= Time.deltaTime; // 冷却时间递减
        if (Input.GetMouseButtonDown(0) && attackTimer <= 0f && movement == Vector3.zero) // 鼠标左键点击且冷却结束时攻击
        {
            animator.SetTrigger("Attack"); // 播放攻击动画
            GetComponent<AudioSource>().Play(); // 播放攻击音效
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation); // 创建子弹实例
            Rigidbody rb = bullet.GetComponent<Rigidbody>(); // 获取子弹刚体
            rb.velocity = transform.forward * bulletSpeed; // 设置子弹速度
            Destroy(bullet, 5f); // 5秒后销毁子弹
            attackTimer = attackCooldown; // 重置攻击冷却计时器
        }

        // 检查失败条件
        if (transform.position.y < -5 || slider.value < 0.01f) // 如果玩家掉落或生命值为0
        {
            failPanel.SetActive(true); // 显示失败面板
            Cursor.lockState = CursorLockMode.None; // 解锁鼠标
            isEnd = true; // 游戏结束
        }

        // 更新剩余敌人数量
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
        remainText.text = "待消灭：" + enemys.Length;

        // 检查是否胜利
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); // 查找所有敌人
        if (enemies.Length == 0) // 如果没有敌人
        {
            winPanel.gameObject.SetActive(true); // 显示胜利面板
            Cursor.lockState = CursorLockMode.None; // 解锁鼠标
            isEnd = true; // 游戏结束
        }
    }
}
