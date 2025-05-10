using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyWeaponBase : WeaponBase
{
    protected Transform _player;
    protected float _lastAttackTime;
    
    protected EnemyWeaponConfig EnemyConfig => config as EnemyWeaponConfig;

    protected override void Start()
    {
        base.Start();
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (_player == null) Debug.LogError("Player reference missing!");
    }

    protected virtual bool CanAttack()
    {
        if (_player == null) return false;

        float distance = Vector3.Distance(transform.position, _player.position);
        bool inRange = distance < EnemyConfig.aiAttackRange;
        bool canFire = Time.time - _lastAttackTime >= config.fireRate;

        return inRange && canFire;
    }

    protected override void Update()
    {
        if (CanAttack())
        {
            TryShoot();
            _lastAttackTime = Time.time; // ÷ÿ÷√º∆ ±∆˜
        }
    }
}