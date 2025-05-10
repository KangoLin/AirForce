using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyState
{
    protected EnemyBase enemy;
    protected EnemyConfig config; // [!code ++]

    public virtual void OnInit(EnemyBase owner)
    {
        enemy = owner ?? throw new System.ArgumentNullException(nameof(owner));
        config = owner.Config; // [!code ++]
    }

    public virtual void OnEnter() { }
    public virtual void OnUpdate() { }
    public virtual void OnFixedUpdate() { }
    public virtual void OnExit() { }
}