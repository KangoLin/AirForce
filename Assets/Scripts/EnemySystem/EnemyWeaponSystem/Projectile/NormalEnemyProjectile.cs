using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemyProjectile : EnemyProjectile
{
    protected override void ReturnToPool()
    {
        NormalEnemyProjectilePool.Instance.Return(this);
    }
}