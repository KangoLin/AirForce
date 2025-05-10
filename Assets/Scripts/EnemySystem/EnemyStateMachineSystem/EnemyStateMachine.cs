using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyStateMachine : MonoBehaviour
{
    private Dictionary<Type, EnemyState> states = new();
    public EnemyState CurrentState { get; private set; } // 正确属性名称
    public int GetStatesCount() => states.Count; // [!code ++]

    public void AddState(EnemyState state)
    {
        if (state == null) return;
        states[state.GetType()] = state;
    }

    public void ChangeState<T>() where T : EnemyState
    {
        // 添加更严格的检查 [!code ++]
        if (states == null || states.Count == 0)
        {
            Debug.LogError("States not initialized!");
            return;
        }

        if (!states.ContainsKey(typeof(T)))
        {
            Debug.LogError($"State {typeof(T)} not registered!");
            return;
        }

        CurrentState?.OnExit();
        CurrentState = states[typeof(T)];
        CurrentState?.OnEnter();
    }

    private void Update()
    {
        CurrentState?.OnUpdate(); // 正确引用属性 [!code ++]
    }

    private void FixedUpdate()
    {
        CurrentState?.OnFixedUpdate(); // 正确引用属性 [!code ++]
    }
}