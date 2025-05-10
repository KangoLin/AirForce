using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Base Config")]
public class EnemyConfig : ScriptableObject
{
    [Header("移动参数")]
    public float maxSpeed = 10f;
    public float acceleration = 5f;
    public float rotationSpeed = 120f;
    public float stopDistance = 3f;

    [Header("物理参数")]
    public float drag = 1f;
    public float angularDrag = 2f;
    public ForceMode forceMode = ForceMode.Acceleration;
    public float brakingForce = 8f;

    // [!code --]
    // public float attackInterval = 2f; 
    // 攻击间隔由武器系统统一管理
}