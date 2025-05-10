using System.Collections;
using UnityEngine;

public class MachineGun : WeaponBase
{
    private MachineGunConfig mgConfig;

    protected override void Start()
    {
        base.Start();
        mgConfig = config as MachineGunConfig;
    }

    protected override void FireProjectile()
    {
        for (int i = 0; i < mgConfig.bulletsPerShot; i++)
        {
            // 修改：使用具体的NormalProjectile类型
            NormalProjectile bullet = ProjectilePool<NormalProjectile>.Instance.Get();
            bullet.transform.SetPositionAndRotation(
                firePoint.position,
                Quaternion.LookRotation(ApplySpread())
            );
            bullet.Initialize(config.projectileConfig, gameObject);
        }
    }

    private Vector3 ApplySpread()
    {
        return Quaternion.Euler(
            Random.Range(-mgConfig.spreadAngle, mgConfig.spreadAngle),
            Random.Range(-mgConfig.spreadAngle, mgConfig.spreadAngle),
            0
        ) * firePoint.forward;
    }
}