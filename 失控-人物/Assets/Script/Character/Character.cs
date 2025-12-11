using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("属性")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    [Header("无敌")]
    public bool invulnerable;

    public float invulnerableDuration;//无敌时间

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        if (invulnerable) //如果无敌则不受伤
        return;
        
        currentHealth -= damage;
        StartCoroutine(nameof(InvulnerableCoroutine));//开始无敌协程
        //StartCoroutine是执行协程的方法，协程是一种可以暂停执行并在稍后继续执行的函数，
        // 常用于处理需要等待的操作，比如等待一段时间或等待某个条件满足。
        //nameof括号表示获取协程的名称作为参数
        if (currentHealth <= 0f)
        {
            //死亡
            Die();
        }
    }
    protected virtual void Die()
    {
        //默认死亡行为
       currentHealth = 0f;
       Destroy(this.gameObject);//销毁角色对象
    }

    //无敌
     protected virtual IEnumerator InvulnerableCoroutine()
    {
        invulnerable = true;
        //等待无敌时间
        yield return new WaitForSeconds(invulnerableDuration);
        invulnerable = false;
    }
    //协程跟updat的区别在于协程可以暂停执行，而update是每帧都执行一次
}
