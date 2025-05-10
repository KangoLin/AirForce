using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("����ֵ����")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("����Ч��")]
    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private float destroyDelay = 2f;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        Debug.Log($"�����˺��¼�����Դ: {damageInfo.DamageSource?.name}");
        currentHealth = Mathf.Max(0, currentHealth - damageInfo.DamageAmount);

        Debug.Log($"{gameObject.name} �ܵ� {damageInfo.DamageAmount} �˺���" +
                 $"ʣ��������{currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        // ����������Ч
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // ������ײ��
        GetComponent<Collider>().enabled = false;

        // �ӳ�����
        Destroy(gameObject, destroyDelay);

        // �������������������߼����������Ʒ�ȣ�
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
#endif
}