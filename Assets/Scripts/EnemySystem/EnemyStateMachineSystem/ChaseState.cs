using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : EnemyState
{
    public override void OnFixedUpdate()
    {
        if (enemy.player == null) return;

        Vector3 toPlayer = enemy.player.position - enemy.transform.position;
        float distance = toPlayer.magnitude;

        if (distance > config.stopDistance)
        {
            Vector3 desiredVelocity = toPlayer.normalized * config.maxSpeed;
            Vector3 force = (desiredVelocity - enemy.rb.velocity) * config.acceleration;
            enemy.rb.AddForce(force, config.forceMode);
        }
        else
        {
            enemy.StateMachine.ChangeState<AttackState>();
        }
    }
}