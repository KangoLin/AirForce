using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : EnemyState
{
    private WeaponBase _weapon; // [!code ++]

    public override void OnEnter()
    {
        // 获取任意类型的武器组件 [!code ++]
        _weapon = enemy.GetComponentInChildren<WeaponBase>();

        if (_weapon == null)
        {
            Debug.LogError($"未找到武器组件: {enemy.gameObject.name}", enemy.gameObject);
            enemy.StateMachine.ChangeState<ChaseState>();
            return;
        }

        if (!(_weapon is EnemyWeaponBase)) // [!code ++]
        {
            Debug.LogError($"武器类型错误，需要继承自EnemyWeaponBase", _weapon.gameObject);
            enemy.StateMachine.ChangeState<ChaseState>();
        }
    }

    public override void OnFixedUpdate()
    {
        if (enemy.player == null) return;

        Vector3 toPlayer = enemy.player.position - enemy.transform.position;
        float distance = toPlayer.magnitude;

        // 使用基类配置参数 [!code ++]
        if (distance > config.stopDistance)
        {
            enemy.StateMachine.ChangeState<ChaseState>();
            return;
        }

        // 通用武器调用 [!code ++]
        (_weapon as EnemyWeaponBase).TryShoot();

        // 使用基类制动参数 [!code ++]
        enemy.rb.AddForce(-enemy.rb.velocity * config.brakingForce, ForceMode.Acceleration);
    }
}