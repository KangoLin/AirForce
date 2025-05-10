using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Projectiles/Missile Config")]
public class MissileConfig : ProjectileConfig
{
    [Header("制导参数")]
    public float turnSpeed = 45f;    // 转向速度(度/秒)
    public float acceleration = 5f;  // 加速度
    public float maxSpeed = 40f;     // 最大速度
}