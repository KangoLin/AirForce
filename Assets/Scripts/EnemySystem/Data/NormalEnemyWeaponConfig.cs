using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Enemy/Normal Config")]
public class NormalEnemyWeaponConfig : EnemyWeaponConfig
{
    // ɾ�� new ������ֱ�Ӽ̳л������ [!code ++]
    [Header("�ӵ�����")]
    public new EnemyProjectileConfig projectileConfig;

    // �������в���
    [Tooltip("�������")]
    public int burstCount = 3;
}