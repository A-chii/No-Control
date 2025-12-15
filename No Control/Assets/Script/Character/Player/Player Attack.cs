using System.Collections;
using UnityEngine;

public class PlayerAttackDetector : MonoBehaviour
{
    [Header("攻击设置")]
    public float attackDuration = 0.3f;        // 攻击检测持续时间
    public int attackDamage = 1;               // 攻击伤害
    public float attackRange = 0.8f;           // 攻击范围
    
    [Header("攻击效果")]
    public GameObject slashEffect;             // 攻击特效
    public AudioClip attackSound;              // 攻击音效
    
    [Header("攻击检测")]
    public Transform attackPoint;              // 攻击检测点（通常是武器位置）
    public LayerMask enemyLayer;               // 敌人层级
    
    // 内部变量
    private bool isAttacking = false;
    private Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        
        // 如果没指定攻击点，使用角色位置
        if (attackPoint == null)
        {
            attackPoint = transform;
        }
    }
    
    // 攻击函数（由玩家控制器调用）
    public void Attack()
    {
        if (isAttacking) return;
        
        StartCoroutine(AttackCoroutine());
    }
    
    // 攻击协程
    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        
        // 播放攻击动画
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        
        // 播放攻击音效
        if (attackSound != null)
        {
            AudioSource.PlayClipAtPoint(attackSound, transform.position);
        }
        
        // 生成攻击特效
        if (slashEffect != null)
        {
            GameObject effect = Instantiate(slashEffect, attackPoint.position, attackPoint.rotation);
            Destroy(effect, 1f); // 1秒后销毁特效
        }
        
        // 等待一小段时间，让动画开始播放
        yield return new WaitForSeconds(0.1f);
        
        // 检测攻击范围内的敌人
        DetectEnemies();
        
        // 等待攻击持续时间结束
        yield return new WaitForSeconds(attackDuration - 0.1f);
        
        isAttacking = false;
    }
    
    // 检测范围内的敌人
    private void DetectEnemies()
    {
        // 圆形检测
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position, 
            attackRange, 
            enemyLayer
        );
        
        // 处理每个被击中的敌人
        foreach (Collider2D enemy in hitEnemies)
        {
            // 计算攻击方向（从玩家指向敌人）
            Vector2 attackDirection = enemy.transform.position - transform.position;
            
            // 调用敌人的受击函数
            EnemyHitDetection enemyHealth = enemy.GetComponent<EnemyHitDetection>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage, attackDirection);
            }
        }
    }
    
    // 在Unity编辑器中绘制攻击范围（便于调试）
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}

