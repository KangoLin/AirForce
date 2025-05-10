using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemy : EnemyBase
{
    protected override void InitializeStates()
    {
        var chaseState = new ChaseState();
        var attackState = new AttackState();

        chaseState.OnInit(this);
        attackState.OnInit(this);

        StateMachine.AddState(chaseState);
        StateMachine.AddState(attackState);
        StateMachine.ChangeState<ChaseState>();
    }
}