using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : EnemyState
{
    private WeaponBase _weapon; // [!code ++]

    public override void OnEnter()
    {
        // ��ȡ�������͵�������� [!code ++]
        _weapon = enemy.GetComponentInChildren<WeaponBase>();

        if (_weapon == null)
        {
            Debug.LogError($"δ�ҵ��������: {enemy.gameObject.name}", enemy.gameObject);
            enemy.StateMachine.ChangeState<ChaseState>();
            return;
        }

        if (!(_weapon is EnemyWeaponBase)) // [!code ++]
        {
            Debug.LogError($"�������ʹ�����Ҫ�̳���EnemyWeaponBase", _weapon.gameObject);
            enemy.StateMachine.ChangeState<ChaseState>();
        }
    }

    public override void OnFixedUpdate()
    {
        if (enemy.player == null) return;

        Vector3 toPlayer = enemy.player.position - enemy.transform.position;
        float distance = toPlayer.magnitude;

        // ʹ�û������ò��� [!code ++]
        if (distance > config.stopDistance)
        {
            enemy.StateMachine.ChangeState<ChaseState>();
            return;
        }

        // ͨ���������� [!code ++]
        (_weapon as EnemyWeaponBase).TryShoot();

        // ʹ�û����ƶ����� [!code ++]
        enemy.rb.AddForce(-enemy.rb.velocity * config.brakingForce, ForceMode.Acceleration);
    }
}