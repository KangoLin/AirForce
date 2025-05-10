using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(menuName = "Projectiles/Base Config")]
public class ProjectileConfig : ScriptableObject
{
    [Header("Base Settings")]
    public float speed = 20f;
    public float lifespan = 3f;
    public int baseDamage = 20;
    public DamageType damageType = DamageType.Kinetic;
    public bool canPenetrate = false;

    [Header("Collision Settings")]
    public bool useAdvancedCollision = false; // 新增参数
    public LayerMask collisionLayers = ~0;    // 默认与所有层碰撞
    public float collisionCheckRadius = 0.5f; // 碰撞检测半径
}
public enum DamageType { Kinetic, Explosive, Energy }