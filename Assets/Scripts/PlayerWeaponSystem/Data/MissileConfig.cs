using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Projectiles/Missile Config")]
public class MissileConfig : ProjectileConfig
{
    [Header("�Ƶ�����")]
    public float turnSpeed = 45f;    // ת���ٶ�(��/��)
    public float acceleration = 5f;  // ���ٶ�
    public float maxSpeed = 40f;     // ����ٶ�
}