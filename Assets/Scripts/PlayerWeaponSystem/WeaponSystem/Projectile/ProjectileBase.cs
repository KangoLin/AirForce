using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    [Header("Collision Settings")]
    [SerializeField] protected LayerMask collisionMask = ~0; // 默认与所有层碰撞
    [SerializeField] protected float collisionCheckRadius = 0.3f;

    protected ProjectileConfig Config { get; private set; }
    protected GameObject Owner { get; private set; }
    protected Rigidbody Rb { get; private set; }

    private RaycastHit[] _hitsCache = new RaycastHit[4];
    protected float _activationTime;

    public virtual void Initialize(ProjectileConfig cfg, GameObject shooter)
    {
        Config = cfg;
        Owner = shooter;
        Rb = GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();

        Rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        Rb.interpolation = RigidbodyInterpolation.Interpolate;

        _activationTime = Time.time;
        Invoke(nameof(Deactivate), Config.lifespan);
    }

    protected virtual void FixedUpdate()
    {
        Move();
        if (Config.useAdvancedCollision)
        {
            CheckContinuousCollision();
        }
    }

    protected virtual void Move()
    {
        if (Rb != null)
        {
            // 添加初始化日志 [!code ++]
            if (Time.time - _activationTime < 0.1f)
            {
                Debug.Log($"子弹初始速度: {Rb.velocity} | 方向: {transform.forward}");
            }
            Rb.velocity = transform.forward * Config.speed;
        }
    }

    private void CheckContinuousCollision()
    {
        int hitCount = Physics.SphereCastNonAlloc(
            transform.position,
            collisionCheckRadius,
            transform.forward,
            _hitsCache,
            Rb.velocity.magnitude * Time.fixedDeltaTime,
            collisionMask
        );

        for (int i = 0; i < hitCount; i++)
        {
            HandleCollision(_hitsCache[i].collider);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other);
    }

    private void HandleCollision(Collider other)
    {
        Debug.Log($"碰撞发生！对象: {other.name} | 层级: {LayerMask.LayerToName(other.gameObject.layer)}"); // [!code ++]
        // 层级过滤
        if ((collisionMask.value & (1 << other.gameObject.layer)) == 0)
            return;

        // 忽略发射者
        if (other.gameObject == Owner)
            return;

        bool shouldDeactivate = true;

        // 伤害判定
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            Debug.Log($"检测到可伤害目标: {other.name}", other.gameObject);
            ApplyDamage(damageable, other.ClosestPoint(transform.position));
            shouldDeactivate = !Config.canPenetrate;
        }
        else
        { // [!code ++]
            Debug.Log($"未找到 IDamageable 组件！目标: {other.name}"); // [!code ++]
        } //

        // 环境碰撞或需要消失的情况
        if (shouldDeactivate)
        {
            Deactivate();
        }

    }

    private void ApplyDamage(IDamageable target, Vector3 hitPoint)
    {
        Debug.Log($"正在施加伤害，数值: {Config.baseDamage}");
        target.TakeDamage(new DamageInfo(
            Config.baseDamage,
            Config.damageType,
            hitPoint,
            Owner
        ));
    }

    public virtual void Deactivate()
    {
        CancelInvoke();
        ReturnToPool();
    }

    protected abstract void ReturnToPool();

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, collisionCheckRadius);
    }
#endif
}