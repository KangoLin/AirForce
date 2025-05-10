using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : WeaponBase
{
    private MissileWeaponConfig missileConfig;

    protected override void Start()
    {
        base.Start();
        missileConfig = config as MissileWeaponConfig;

        if (missileConfig == null)
        {
            Debug.LogError("�����������ʹ��󣡱���ʹ��MissileWeaponConfig");
            return;
        }

        // ����ʱ��������
        missileConfig.missilesPerShot = Mathf.Min(
            missileConfig.missilesPerShot,
            missileConfig.ammoCapacity
        );
        missileConfig.launchSpread = Mathf.Clamp(missileConfig.launchSpread, 0f, 360f);
    }

    protected override void FireProjectile()
    {
        int actualShots = Mathf.Min(
            missileConfig.missilesPerShot,
            currentAmmo
        );

        for (int i = 0; i < actualShots; i++)
        {
            FireSingleMissile(GetLaunchDirection(i, actualShots));
        }

        currentAmmo -= actualShots;
        nextFireTime = Time.time + config.fireRate;
    }

    private Vector3 GetLaunchDirection(int missileIndex, int totalShots)
    {
        if (totalShots <= 1) return firePoint.forward;

        // �Ľ���ɢ���㷨��֧������Ƕȣ�
        float startAngle = -missileConfig.launchSpread / 2f;
        float endAngle = missileConfig.launchSpread / 2f;
        float angleStep = (endAngle - startAngle) / (totalShots - 1);
        float currentAngle = startAngle + angleStep * missileIndex;

        return Quaternion.Euler(0, currentAngle, 0) * firePoint.forward;
    }

    private void FireSingleMissile(Vector3 direction)
    {
        Missile missile = ProjectilePool<Missile>.Instance.Get();
        if (missile == null)
        {
            Debug.LogWarning("���������Ϊ�գ�");
            return;
        }

        missile.transform.SetPositionAndRotation(
            firePoint.position,
            Quaternion.LookRotation(direction.normalized) // ȷ�������׼��
        );
        missile.Initialize(config.projectileConfig, gameObject);

        if (missileConfig.autoLock)
        {
            Transform target = FindClosestEnemy();
            if (target != null)
            {
                missile.SetTarget(target);
            }
        }
    }

    private Transform FindClosestEnemy()
    {
        // ʵ��Ŀ������߼����ֲ���
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies == null || enemies.Length == 0) return null;

        Transform closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 position = firePoint.position;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            float distance = Vector3.Distance(position, enemy.transform.position);
            if (distance < minDistance)
            {
                closest = enemy.transform;
                minDistance = distance;
            }
        }

        return closest;
    }

    protected override IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(config.reloadTime);
        currentAmmo = config.ammoCapacity;
        isReloading = false;
    }
}