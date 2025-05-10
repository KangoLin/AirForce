using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Missile Weapon Config")]
public class MissileWeaponConfig : WeaponConfig
{
    [Header("发射设置")]
    [Tooltip("每次发射的导弹数量")]
    [Min(1)]
    public int missilesPerShot = 4;

    [Tooltip("发射角度散布（度）")]
    [Min(0f)]
    public float launchSpread = 180f; // 现在可以设置为任意正数

    [Header("制导参数")]
    [Tooltip("锁定时间(秒)")]
    public float lockOnTime = 1.2f;

    [Tooltip("是否自动锁定目标")]
    public bool autoLock = true;

    private void OnValidate()
    {
        missilesPerShot = Mathf.Min(missilesPerShot, ammoCapacity);
        launchSpread = Mathf.Clamp(launchSpread, 0f, 360f); // 防止超过物理极限
    }
}