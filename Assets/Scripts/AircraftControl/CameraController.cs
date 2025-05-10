using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("��������")]
    [SerializeField, Tooltip("��׼��Ұ")]
    private float normalFOV = 60f;

    [SerializeField, Tooltip("���ʱ�����Ұ")]
    private float boostFOV = 75f;

    [SerializeField, Tooltip("��Ұ�仯�ٶ�")]
    private float fovChangeSpeed = 5f;

    [Header("���Ч��")]
    [SerializeField, Tooltip("��ͷ����ǿ��")]
    private float stretchIntensity = 0.3f;

    [SerializeField, Tooltip("������Ӧ�ٶ�")]
    private float stretchResponseSpeed = 8f;

    [Header("��������")]
    [SerializeField, Tooltip("λ�ø���ƽ����")]
    private float positionSmoothness = 5f;

    [SerializeField, Tooltip("��׼λ��ƫ��")]
    private Vector3 normalOffset = new Vector3(0, 2, -10);

    [SerializeField, Tooltip("���λ��ƫ��")]
    private Vector3 boostOffset = new Vector3(0, 2, -12);

    [Header("��Ч��")]
    [SerializeField, Tooltip("������ǿ��(��Ϊ0����ȫ���÷ǳ����)")]
    private float baseShakeIntensity = 0f; // �޸�Ϊ0��Ĭ�Ͻ��÷ǳ����

    [SerializeField, Tooltip("�����ǿ��")]
    private float boostShakeIntensity = 0.15f;

    [SerializeField, Tooltip("��Ƶ��")]
    private float shakeFrequency = 10f;

    [SerializeField, Tooltip("��˥���ٶ�")]
    private float shakeDamping = 3f;

    [SerializeField, Tooltip("��С�ٶȰٷֱȲſ�ʼ��(0-1)")]
    private float minSpeedForShake = 0.5f; // ��������

    // ˽�б���
    private Camera cam;
    private AircraftController aircraftController;
    private float currentFOV;
    private float currentStretch;
    private Vector3 currentOffset;
    private Vector3 originalPosition;
    private float shakeTimer;
    private float currentShakeIntensity;

    void Awake()
    {
        cam = GetComponent<Camera>();
        currentFOV = normalFOV;
        currentOffset = normalOffset;
        originalPosition = transform.localPosition;
    }

    void Start()
    {
        aircraftController = GetComponentInParent<AircraftController>();
        if (aircraftController == null)
        {
            Debug.LogError("CameraController: δ�ڸ����ҵ�AircraftController!");
        }
    }

    void LateUpdate()
    {
        if (aircraftController == null) return;

        HandleBoostEffects();
        HandleCameraFollow();
        HandleCameraShake();
    }

    private void HandleBoostEffects()
    {
        // ����FOV�仯
        float targetFOV = aircraftController.IsBoosting ? boostFOV : normalFOV;
        currentFOV = Mathf.Lerp(currentFOV, targetFOV, fovChangeSpeed * Time.deltaTime);
        cam.fieldOfView = currentFOV;

        // ����ͷ����Ч��
        float targetStretch = aircraftController.IsBoosting ? stretchIntensity : 0f;
        currentStretch = Mathf.Lerp(currentStretch, targetStretch, stretchResponseSpeed * Time.deltaTime);

        // ����λ��ƫ��
        Vector3 targetOffset = aircraftController.IsBoosting ? boostOffset : normalOffset;
        currentOffset = Vector3.Lerp(currentOffset, targetOffset, fovChangeSpeed * Time.deltaTime);

        // ���ڳ��ʱ�����ʱ������
        if (aircraftController.IsBoosting || aircraftController.SpeedPercent > minSpeedForShake)
        {
            float targetIntensity = aircraftController.IsBoosting ? boostShakeIntensity : baseShakeIntensity;
            currentShakeIntensity = Mathf.Lerp(currentShakeIntensity, targetIntensity, 5f * Time.deltaTime);
        }
        else
        {
            currentShakeIntensity = Mathf.Lerp(currentShakeIntensity, 0f, shakeDamping * Time.deltaTime);
        }
    }

    private void HandleCameraFollow()
    {
        if (transform.parent == null) return;

        // λ�ø���
        Vector3 targetPosition = transform.parent.TransformPoint(currentOffset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, positionSmoothness * Time.deltaTime);

        // �������ʼ�տ�������ǰ��
        transform.rotation = Quaternion.LookRotation(transform.parent.forward, transform.parent.up);

        // Ӧ������Ч��
        if (currentStretch > 0.01f)
        {
            transform.position += transform.forward * currentStretch;
        }
    }

    private void HandleCameraShake()
    {
        // ����ǿ�Ⱥ�Сʱֱ����������
        if (currentShakeIntensity <= 0.01f) return;

        // �����𶯼�ʱ��
        shakeTimer += Time.deltaTime * shakeFrequency;

        // ʹ��Perlin��������ƽ���������
        float shakeX = Mathf.PerlinNoise(shakeTimer, 0) * 2 - 1;
        float shakeY = Mathf.PerlinNoise(0, shakeTimer) * 2 - 1;
        float shakeZ = Mathf.PerlinNoise(shakeTimer, shakeTimer) * 2 - 1;

        // Ӧ����(���ڱ��ؿռ�)
        Vector3 shakeOffset = new Vector3(shakeX, shakeY, shakeZ) * currentShakeIntensity * aircraftController.SpeedPercent;
        transform.localPosition += shakeOffset;
    }
}