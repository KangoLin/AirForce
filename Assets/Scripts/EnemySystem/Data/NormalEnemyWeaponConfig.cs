using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Enemy/Normal Config")]
public class NormalEnemyWeaponConfig : EnemyWeaponConfig
{
    // 删除 new 声明，直接继承基类参数 [!code ++]
    [Header("子弹设置")]
    public new EnemyProjectileConfig projectileConfig;

    // 其他特有参数
    [Tooltip("连射次数")]
    public int burstCount = 3;
}