using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Base Config")]
public class WeaponConfig : ScriptableObject
{
    [Header("��������")]
    public float fireRate = 0.1f;
    public int ammoCapacity = 30;
    public float reloadTime = 1.5f;
    public int baseDamage = 20; // ���������˺��ֶ�
    public ProjectileConfig projectileConfig;
}