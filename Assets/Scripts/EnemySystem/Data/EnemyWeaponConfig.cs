using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Enemy/Config")]
public class EnemyWeaponConfig : WeaponConfig
{
    [Header("近战专用")]
    public float meleeAttackRange = 3f;

    [Header("远程专用")]
    public float aiAttackRange = 10f;

    // 使用虚属性代替字段 [!code ++]
    public virtual float FireRate => fireRate; // [!code ++]
}