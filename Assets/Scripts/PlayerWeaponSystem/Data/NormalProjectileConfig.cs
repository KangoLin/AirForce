using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Projectiles/Normal Config")]
public class NormalProjectileConfig : ProjectileConfig
{
    [Header("Physics Settings")]
    public bool useGravity = false;
    [Range(0, 5)] public float gravityScale = 1f;
    [Min(0)] public float drag = 0.1f;
}