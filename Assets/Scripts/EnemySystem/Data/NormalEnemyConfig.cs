using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Normal Config")]
public class NormalEnemyConfig : EnemyConfig
{
    [Header("特殊攻击参数")]
    public float cooldownTime = 1f;
    public int burstCount = 3;
    public float spreadAngle = 5f;

    // [!code --]
    // public new float attackInterval = 2f; 
    // 移除重复声明
}