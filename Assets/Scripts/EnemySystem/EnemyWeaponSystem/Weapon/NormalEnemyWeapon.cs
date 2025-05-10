using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemyWeapon : EnemyWeaponBase
{
    protected override void FireProjectile()
    {
        if (config.projectileConfig == null)
        {
            Debug.LogError("◊”µØ≈‰÷√»± ß!", gameObject);
            return;
        }

        var projectile = NormalEnemyProjectilePool.Instance.Get();
        projectile.transform.SetPositionAndRotation(
            firePoint.position,
            firePoint.rotation
        );

        // ÷ÿ÷√ŒÔ¿Ì◊¥Ã¨
        if (projectile.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        projectile.Initialize(config.projectileConfig, gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePoint.position, 0.1f);
            Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.forward);
        }
    }
#endif
}