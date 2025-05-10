using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    [Header("Collision Settings")]
    [SerializeField] protected LayerMask collisionMask = ~0; // Ĭ�������в���ײ
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
            // ��ӳ�ʼ����־ [!code ++]
            if (Time.time - _activationTime < 0.1f)
            {
                Debug.Log($"�ӵ���ʼ�ٶ�: {Rb.velocity} | ����: {transform.forward}");
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
        Debug.Log($"��ײ����������: {other.name} | �㼶: {LayerMask.LayerToName(other.gameObject.layer)}"); // [!code ++]
        // �㼶����
        if ((collisionMask.value & (1 << other.gameObject.layer)) == 0)
            return;

        // ���Է�����
        if (other.gameObject == Owner)
            return;

        bool shouldDeactivate = true;

        // �˺��ж�
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            Debug.Log($"��⵽���˺�Ŀ��: {other.name}", other.gameObject);
            ApplyDamage(damageable, other.ClosestPoint(transform.position));
            shouldDeactivate = !Config.canPenetrate;
        }
        else
        { // [!code ++]
            Debug.Log($"δ�ҵ� IDamageable �����Ŀ��: {other.name}"); // [!code ++]
        } //

        // ������ײ����Ҫ��ʧ�����
        if (shouldDeactivate)
        {
            Deactivate();
        }

    }

    private void ApplyDamage(IDamageable target, Vector3 hitPoint)
    {
        Debug.Log($"����ʩ���˺�����ֵ: {Config.baseDamage}");
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