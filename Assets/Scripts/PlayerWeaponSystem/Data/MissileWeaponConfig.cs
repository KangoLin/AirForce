using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Missile Weapon Config")]
public class MissileWeaponConfig : WeaponConfig
{
    [Header("��������")]
    [Tooltip("ÿ�η���ĵ�������")]
    [Min(1)]
    public int missilesPerShot = 4;

    [Tooltip("����Ƕ�ɢ�����ȣ�")]
    [Min(0f)]
    public float launchSpread = 180f; // ���ڿ�������Ϊ��������

    [Header("�Ƶ�����")]
    [Tooltip("����ʱ��(��)")]
    public float lockOnTime = 1.2f;

    [Tooltip("�Ƿ��Զ�����Ŀ��")]
    public bool autoLock = true;

    private void OnValidate()
    {
        missilesPerShot = Mathf.Min(missilesPerShot, ammoCapacity);
        launchSpread = Mathf.Clamp(launchSpread, 0f, 360f); // ��ֹ����������
    }
}