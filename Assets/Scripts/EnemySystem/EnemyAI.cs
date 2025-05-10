using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyAI : MonoBehaviour
{
    [Header("移动参数")]
    [SerializeField] private float maxSpeed = 10f;     // 最大移动速度
    [SerializeField] private float acceleration = 5f;  // 加速度
    [SerializeField] private float rotationSpeed = 2f; // 转向速度系数
    [SerializeField] private float stopDistance = 3f;  // 停止距离
    [SerializeField] private float brakingForce = 8f;  // 制动力量

    [Header("物理参数")]
    [SerializeField] private float drag = 1f;          // 线性阻力
    [SerializeField] private float angularDrag = 2f;   // 角阻力
    [SerializeField] private ForceMode forceMode = ForceMode.Acceleration;

    [Header("目标追踪")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private LayerMask obstacleLayers;

    private Rigidbody _rb;
    private Transform _player;
    private Vector3 _currentVelocity;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        ConfigureRigidbody();
        FindPlayer();
    }

    void FixedUpdate()
    {
        if (_player == null) return;

        Vector3 toPlayer = _player.position - transform.position;
        float distance = toPlayer.magnitude;

        HandleMovement(toPlayer, distance);
        HandleRotation(toPlayer);
    }

    void ConfigureRigidbody()
    {
        _rb.drag = drag;
        _rb.angularDrag = angularDrag;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj) _player = playerObj.transform;
    }

    void HandleMovement(Vector3 direction, float distance)
    {
        // 根据距离调整速度
        float targetSpeed = Mathf.Lerp(0, maxSpeed, Mathf.InverseLerp(stopDistance, stopDistance * 2, distance));

        // 计算加速度
        Vector3 targetVelocity = direction.normalized * targetSpeed;
        Vector3 force = (targetVelocity - _rb.velocity) * acceleration;

        // 应用物理力
        _rb.AddForce(force, forceMode);

        // 制动逻辑
        if (distance < stopDistance)
        {
            _rb.AddForce(-_rb.velocity * brakingForce, ForceMode.Acceleration);
        }
    }

    void HandleRotation(Vector3 direction)
    {
        // 使用物理旋转
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Quaternion rotationStep = Quaternion.RotateTowards(
            _rb.rotation,
            targetRotation,
            rotationSpeed * 100 * Time.fixedDeltaTime
        );

        _rb.MoveRotation(rotationStep);
    }

    void OnCollisionStay(Collision collision)
    {
        // 防卡墙逻辑
        if ((obstacleLayers.value & (1 << collision.gameObject.layer)) != 0)
        {
            Vector3 avoidDirection = (transform.position - collision.contacts[0].point).normalized;
            _rb.AddForce(avoidDirection * acceleration * 2, ForceMode.Acceleration);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (_player != null)
        {
            Gizmos.color = Color.Lerp(Color.red, Color.green, _rb.velocity.magnitude / maxSpeed);
            Gizmos.DrawLine(transform.position, _player.position);
            Gizmos.DrawWireSphere(_player.position, stopDistance);
        }
    }
#endif
}