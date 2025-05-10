using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Projectiles/Enemy/Normal Enemy Config")]
public class NormalEnemyProjectileConfig : EnemyProjectileConfig
{
    [Header("高级追踪设置")]
    [Tooltip("最大转向角度(度/秒)")]
    public float maxTurnRate = 90f;
}