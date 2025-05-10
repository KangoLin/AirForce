using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : ProjectileBase
{
    private MissileConfig MissileConfig => Config as MissileConfig;
    private Transform _target;
    private float _currentSpeed;

    public override void Initialize(ProjectileConfig cfg, GameObject shooter)
    {
        base.Initialize(cfg, shooter);

        if (MissileConfig == null)
        {
            Debug.LogError("配置类型错误！必须使用MissileConfig");
            return;
        }

        _currentSpeed = MissileConfig.speed;
    }

    public void SetTarget(Transform newTarget) => _target = newTarget;

    protected override void Move()
    {
        if (_target != null)
        {
            Vector3 direction = (_target.position - transform.position).normalized;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(direction),
                MissileConfig.turnSpeed * Time.fixedDeltaTime
            );
        }

        _currentSpeed = Mathf.Min(
            _currentSpeed + (MissileConfig.acceleration * Time.fixedDeltaTime),
            MissileConfig.maxSpeed
        );

        Rb.velocity = transform.forward * _currentSpeed;
    }

    protected override void ReturnToPool()
    {
        _target = null;
        _currentSpeed = 0f;

        if (ProjectilePool<Missile>.Instance != null)
        {
            ProjectilePool<Missile>.Instance.Return(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}