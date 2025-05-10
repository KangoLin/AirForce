using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyProjectile : ProjectileBase
{
    protected Transform _playerTransform;
    protected EnemyProjectileConfig EnemyConfig => Config as EnemyProjectileConfig;
    private bool _isFirstFrame = true; // [!code ++]

    public override void Initialize(ProjectileConfig cfg, GameObject shooter)
    {
        base.Initialize(cfg, shooter);
        if (EnemyConfig != null && EnemyConfig.homing)
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
    }

    protected override void Move()
    {
        // 确保基础移动先执行 [!code ++]
        base.Move();

        if (ShouldApplyHoming())
        {
            ApplyHomingForce();
        }
    }

    private bool ShouldApplyHoming() // [!code ++]
    {
        return EnemyConfig != null &&
               EnemyConfig.homing &&
               _playerTransform != null &&
               !_isFirstFrame; // 跳过第一帧 [!code ++]
    }

    private void ApplyHomingForce() // [!code ++]
    {
        Vector3 desiredDirection = (_playerTransform.position - transform.position).normalized;
        Vector3 currentDirection = Rb.velocity.normalized;

        // 使用矢量差值平滑转向
        Vector3 steering = Vector3.Lerp(
            currentDirection,
            desiredDirection,
            EnemyConfig.homingForce * Time.fixedDeltaTime
        );

        Rb.velocity = steering * Rb.velocity.magnitude;
    }

    protected override void FixedUpdate() // [!code ++]
    {
        base.FixedUpdate();
        _isFirstFrame = false; // 标记第一帧结束
    }
}