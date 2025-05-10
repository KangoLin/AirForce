using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyAI : MonoBehaviour
{
    [Header("�ƶ�����")]
    [SerializeField] private float maxSpeed = 10f;     // ����ƶ��ٶ�
    [SerializeField] private float acceleration = 5f;  // ���ٶ�
    [SerializeField] private float rotationSpeed = 2f; // ת���ٶ�ϵ��
    [SerializeField] private float stopDistance = 3f;  // ֹͣ����
    [SerializeField] private float brakingForce = 8f;  // �ƶ�����

    [Header("�������")]
    [SerializeField] private float drag = 1f;          // ��������
    [SerializeField] private float angularDrag = 2f;   // ������
    [SerializeField] private ForceMode forceMode = ForceMode.Acceleration;

    [Header("Ŀ��׷��")]
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
        // ���ݾ�������ٶ�
        float targetSpeed = Mathf.Lerp(0, maxSpeed, Mathf.InverseLerp(stopDistance, stopDistance * 2, distance));

        // ������ٶ�
        Vector3 targetVelocity = direction.normalized * targetSpeed;
        Vector3 force = (targetVelocity - _rb.velocity) * acceleration;

        // Ӧ��������
        _rb.AddForce(force, forceMode);

        // �ƶ��߼�
        if (distance < stopDistance)
        {
            _rb.AddForce(-_rb.velocity * brakingForce, ForceMode.Acceleration);
        }
    }

    void HandleRotation(Vector3 direction)
    {
        // ʹ��������ת
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
        // ����ǽ�߼�
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