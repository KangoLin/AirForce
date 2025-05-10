using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileConfig : ProjectileConfig
{
    [Header("Enemy Base")]
    public bool homing = false;
    public float homingForce = 5f;
}