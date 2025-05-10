using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("生命值设置")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("死亡效果")]
    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private float destroyDelay = 2f;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        Debug.Log($"接收伤害事件，来源: {damageInfo.DamageSource?.name}");
        currentHealth = Mathf.Max(0, currentHealth - damageInfo.DamageAmount);

        Debug.Log($"{gameObject.name} 受到 {damageInfo.DamageAmount} 伤害，" +
                 $"剩余生命：{currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        // 播放死亡特效
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // 禁用碰撞体
        GetComponent<Collider>().enabled = false;

        // 延迟销毁
        Destroy(gameObject, destroyDelay);

        // 这里可以添加其他死亡逻辑（如掉落物品等）
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
#endif
}