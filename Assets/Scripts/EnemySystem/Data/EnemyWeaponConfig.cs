using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Enemy/Config")]
public class EnemyWeaponConfig : WeaponConfig
{
    [Header("��սר��")]
    public float meleeAttackRange = 3f;

    [Header("Զ��ר��")]
    public float aiAttackRange = 10f;

    // ʹ�������Դ����ֶ� [!code ++]
    public virtual float FireRate => fireRate; // [!code ++]
}