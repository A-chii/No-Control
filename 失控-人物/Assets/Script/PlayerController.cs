using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // 必须引用

public class PlayerController : MonoBehaviour
{
    // 不建议public，除非你需要外部访问。一般用private并在Awake初始化
    private InputActions inputActions; 
    
    [Header("Settings")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput; // 存储输入向量

    // 状态标记
    public bool isMeleeAttack;

    private void Awake()
    {
        // 1. 初始化 InputActions 实例
        inputActions = new InputActions();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // 2. 启用输入系统
        inputActions.Gameplay.Enable();
        // 注册攻击事件
        inputActions.Gameplay.MeleeAttack.started += OnMeleeAttack;
    }

    private void OnDisable()
    {
        // 养成好习惯：禁用时取消注册，防止内存泄漏
        inputActions.Gameplay.MeleeAttack.started -= OnMeleeAttack;
        inputActions.Gameplay.Disable();
    }

    private void Update()
    {
        // 3. 在 Update 中读取输入 (新系统写法)
        // 获取 Move Action 的 Vector2 值
        moveInput = inputActions.Gameplay.Move.ReadValue<Vector2>();

        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        // 4. 处理镜像翻转
        // 使用 mathf.Abs 容错，防止为0时翻转
        if (moveInput.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (moveInput.x > 0.01f)
            transform.localScale = new Vector3(1, 1, 1);

        // 5. 应用移动 (Input System 的值通常已经是 normalized 的，如果是键盘则不是，这里加 normalized 保险)
        // 注意：如果是手柄摇杆，ReadValue通常已经处理过范围，这里简单处理即可
        rb.velocity = moveInput * moveSpeed;
    }

    private void UpdateAnimation()
    {
        animator.SetFloat("Horizontal", moveInput.x);
        animator.SetFloat("Vertical", moveInput.y);
        // 使用 magnitude 即可
        animator.SetFloat("Speed", moveInput.sqrMagnitude);
        
        // 这里的 bool 设置通常需要配合动画机逻辑，如果只是触发攻击，SetTrigger 足够
        // 如果你需要这个 bool 来控制混合树，请确保有地方把它设回 false
        animator.SetBool("isMeleeAttack", isMeleeAttack);
    }

    // 6. 修正回调参数类型
    private void OnMeleeAttack(InputAction.CallbackContext context)
    {
        // 防止攻击时重复触发，或者可以在这里加冷却时间
        if (!isMeleeAttack)
        {
            animator.SetTrigger("MeleeAttack");
            isMeleeAttack = true;
            
            // 7. 临时解决方案：简单的协程重置状态 (更好的做法是用 Animation Event)
            // 如果你的攻击动画是 0.5秒，这里可以暂时写死，或者等待动画结束
            StartCoroutine(ResetAttackState(0.5f)); 
        }
    }

    IEnumerator ResetAttackState(float time)
    {
        yield return new WaitForSeconds(time);
        isMeleeAttack = false;
    }
}