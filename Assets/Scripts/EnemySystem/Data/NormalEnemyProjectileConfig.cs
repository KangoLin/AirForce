using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Projectiles/Enemy/Normal Enemy Config")]
public class NormalEnemyProjectileConfig : EnemyProjectileConfig
{
    [Header("�߼�׷������")]
    [Tooltip("���ת��Ƕ�(��/��)")]
    public float maxTurnRate = 90f;
}