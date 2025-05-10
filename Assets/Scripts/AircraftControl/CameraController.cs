using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("基本设置")]
    [SerializeField, Tooltip("标准视野")]
    private float normalFOV = 60f;

    [SerializeField, Tooltip("冲刺时最大视野")]
    private float boostFOV = 75f;

    [SerializeField, Tooltip("视野变化速度")]
    private float fovChangeSpeed = 5f;

    [Header("冲刺效果")]
    [SerializeField, Tooltip("镜头拉伸强度")]
    private float stretchIntensity = 0.3f;

    [SerializeField, Tooltip("拉伸响应速度")]
    private float stretchResponseSpeed = 8f;

    [Header("跟随设置")]
    [SerializeField, Tooltip("位置跟随平滑度")]
    private float positionSmoothness = 5f;

    [SerializeField, Tooltip("标准位置偏移")]
    private Vector3 normalOffset = new Vector3(0, 2, -10);

    [SerializeField, Tooltip("冲刺位置偏移")]
    private Vector3 boostOffset = new Vector3(0, 2, -12);

    [Header("震动效果")]
    [SerializeField, Tooltip("基础震动强度(设为0可完全禁用非冲刺震动)")]
    private float baseShakeIntensity = 0f; // 修改为0，默认禁用非冲刺震动

    [SerializeField, Tooltip("冲刺震动强度")]
    private float boostShakeIntensity = 0.15f;

    [SerializeField, Tooltip("震动频率")]
    private float shakeFrequency = 10f;

    [SerializeField, Tooltip("震动衰减速度")]
    private float shakeDamping = 3f;

    [SerializeField, Tooltip("最小速度百分比才开始震动(0-1)")]
    private float minSpeedForShake = 0.5f; // 新增参数

    // 私有变量
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
            Debug.LogError("CameraController: 未在父级找到AircraftController!");
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
        // 处理FOV变化
        float targetFOV = aircraftController.IsBoosting ? boostFOV : normalFOV;
        currentFOV = Mathf.Lerp(currentFOV, targetFOV, fovChangeSpeed * Time.deltaTime);
        cam.fieldOfView = currentFOV;

        // 处理镜头拉伸效果
        float targetStretch = aircraftController.IsBoosting ? stretchIntensity : 0f;
        currentStretch = Mathf.Lerp(currentStretch, targetStretch, stretchResponseSpeed * Time.deltaTime);

        // 处理位置偏移
        Vector3 targetOffset = aircraftController.IsBoosting ? boostOffset : normalOffset;
        currentOffset = Vector3.Lerp(currentOffset, targetOffset, fovChangeSpeed * Time.deltaTime);

        // 仅在冲刺时或高速时启用震动
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

        // 位置跟随
        Vector3 targetPosition = transform.parent.TransformPoint(currentOffset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, positionSmoothness * Time.deltaTime);

        // 保持相机始终看向父物体前方
        transform.rotation = Quaternion.LookRotation(transform.parent.forward, transform.parent.up);

        // 应用拉伸效果
        if (currentStretch > 0.01f)
        {
            transform.position += transform.forward * currentStretch;
        }
    }

    private void HandleCameraShake()
    {
        // 当震动强度很小时直接跳过计算
        if (currentShakeIntensity <= 0.01f) return;

        // 更新震动计时器
        shakeTimer += Time.deltaTime * shakeFrequency;

        // 使用Perlin噪声生成平滑的随机震动
        float shakeX = Mathf.PerlinNoise(shakeTimer, 0) * 2 - 1;
        float shakeY = Mathf.PerlinNoise(0, shakeTimer) * 2 - 1;
        float shakeZ = Mathf.PerlinNoise(shakeTimer, shakeTimer) * 2 - 1;

        // 应用震动(基于本地空间)
        Vector3 shakeOffset = new Vector3(shakeX, shakeY, shakeZ) * currentShakeIntensity * aircraftController.SpeedPercent;
        transform.localPosition += shakeOffset;
    }
}