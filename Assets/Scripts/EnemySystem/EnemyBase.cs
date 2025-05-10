using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] protected EnemyConfig config;
    protected internal Rigidbody rb;
    protected internal Transform player;

    public EnemyStateMachine StateMachine { get; private set; }
    public EnemyConfig Config => config;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        StateMachine = gameObject.AddComponent<EnemyStateMachine>();
        ConfigurePhysics();
    }

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        InitializeStates();
    }

    protected virtual void ConfigurePhysics()
    {
        rb.drag = config.drag;
        rb.angularDrag = config.angularDrag;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    protected internal virtual void UpdateRotation()
    {
        if (player == null) return;

        Vector3 toPlayer = (player.position - transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(toPlayer);
        rb.MoveRotation(Quaternion.RotateTowards(
            rb.rotation,
            targetRot,
            config.rotationSpeed * Time.fixedDeltaTime
        ));
    }

    protected abstract void InitializeStates();

    protected virtual void FixedUpdate()
    {
        UpdateRotation();
    }
}