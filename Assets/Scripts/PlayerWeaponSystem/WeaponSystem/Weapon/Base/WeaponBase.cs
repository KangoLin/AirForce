using System.Collections;
using System.Collections.Generic;
// WeaponBase.cs
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public enum WeaponType { Ranged, Melee }

    [Header("基础配置")]
    [SerializeField] protected WeaponConfig config;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected WeaponType weaponType = WeaponType.Ranged;

    protected int currentAmmo;
    protected bool isReloading;
    protected float nextFireTime;
    protected Animator _animator;

    protected virtual void Start()
    {
        ValidateConfig();
        currentAmmo = config.ammoCapacity;
        _animator = GetComponent<Animator>();
    }
    protected virtual void Update()
    {
        // 基类默认空实现
    }
    void ValidateConfig()
    {
        if (config == null || (weaponType == WeaponType.Ranged && firePoint == null))
            throw new System.Exception("武器配置不完整");
    }

    public virtual void TryShoot()
    {
        if (Time.time < nextFireTime || isReloading) return;

        switch (weaponType)
        {
            case WeaponType.Ranged:
                HandleRangedAttack();
                break;
            case WeaponType.Melee:
                HandleMeleeAttack();
                break;
        }
    }

    protected virtual void HandleRangedAttack()
    {
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        FireProjectile();
        UpdateAmmo();
        nextFireTime = Time.time + config.fireRate;

        if (_animator != null)
            _animator.SetTrigger("Fire");
    }

    protected virtual void HandleMeleeAttack()
    {
        MeleeAttack();
        nextFireTime = Time.time + config.fireRate;

        if (_animator != null)
            _animator.SetTrigger("Melee");
    }

    protected abstract void FireProjectile();
    protected virtual void MeleeAttack() { } // 新增虚方法

    // 新增：统一的弹药更新逻辑
    protected virtual void UpdateAmmo()
    {
        currentAmmo = Mathf.Max(0, currentAmmo - 1);
    }
    protected virtual IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(config.reloadTime);
        currentAmmo = config.ammoCapacity;
        isReloading = false;
    }

    public int GetCurrentAmmo() => currentAmmo;
}