using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Normal Config")]
public class NormalEnemyConfig : EnemyConfig
{
    [Header("���⹥������")]
    public float cooldownTime = 1f;
    public int burstCount = 3;
    public float spreadAngle = 5f;

    // [!code --]
    // public new float attackInterval = 2f; 
    // �Ƴ��ظ�����
}