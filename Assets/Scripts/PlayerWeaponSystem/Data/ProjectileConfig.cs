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
    public bool useAdvancedCollision = false; // ��������
    public LayerMask collisionLayers = ~0;    // Ĭ�������в���ײ
    public float collisionCheckRadius = 0.5f; // ��ײ���뾶
}
public enum DamageType { Kinetic, Explosive, Energy }