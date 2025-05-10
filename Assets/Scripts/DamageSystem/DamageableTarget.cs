using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableTarget : MonoBehaviour, IDamageable
{
    [Header("ÉúÃüÖµ")]
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _currentHealth;

    private void Start() => ResetHealth();

    public void ResetHealth() => _currentHealth = _maxHealth;

    public void TakeDamage(DamageInfo damageInfo)
    {
        _currentHealth = Mathf.Max(0, _currentHealth - damageInfo.DamageAmount);

        if (_currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _maxHealth = Mathf.Max(1, _maxHealth);
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
    }
#endif
}