using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalProjectile : ProjectileBase
{
    private NormalProjectileConfig NormalConfig => Config as NormalProjectileConfig;

    public override void Initialize(ProjectileConfig cfg, GameObject shooter)
    {
        base.Initialize(cfg, shooter);

        if (NormalConfig == null)
        {
            Debug.LogError("配置类型错误！必须使用NormalProjectileConfig");
            return;
        }

        Rb.useGravity = NormalConfig.useGravity;
        Rb.drag = NormalConfig.drag;
    }

    protected override void Move()
    {
        base.Move();

        if (NormalConfig.useGravity && NormalConfig.gravityScale > 0)
        {
            Rb.AddForce(Physics.gravity * NormalConfig.gravityScale, ForceMode.Acceleration);
        }
    }

    protected override void ReturnToPool()
    {
        if (ProjectilePool<NormalProjectile>.Instance != null)
        {
            ProjectilePool<NormalProjectile>.Instance.Return(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}