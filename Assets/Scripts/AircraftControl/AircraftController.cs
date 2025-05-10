using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AircraftController : MonoBehaviour
{
    // =============== 核心运动参数 ===============
    [Header("MOVEMENT SETTINGS")]
    [SerializeField, Tooltip("基础加速度（米/秒?）")]
    private float acceleration = 25f;

    [SerializeField, Tooltip("最大移动速度（米/秒）")]
    private float maxSpeed = 100f;

    [SerializeField, Tooltip("垂直速度限制（米/秒）")]
    private float verticalSpeedLimit = 50f;

    [SerializeField, Tooltip("自然减速度（米/秒?）")]
    private float naturalDeceleration = 5f;

    [SerializeField, Tooltip("刹车减速度（米/秒?）")]
    private float brakeDeceleration = 20f;

    // =============== 冲刺系统 ===============
    [Header("BOOST SYSTEM")]
    [SerializeField, Tooltip("冲刺速度倍率")]
    private float boostMultiplier = 1.5f;

    [SerializeField, Tooltip("冲刺加速度")]
    private float boostAcceleration = 50f;

    // =============== 能量系统 ===============
    [Header("ENERGY SYSTEM")]
    [SerializeField, Tooltip("能量最大值")]
    private float maxEnergy = 100f;

    [SerializeField, Tooltip("冲刺能量消耗/秒")]
    private float energyConsumptionRate = 5f;

    [SerializeField, Tooltip("能量恢复/秒")]
    private float energyRecoveryRate = 3f;

    [SerializeField, Tooltip("最低冲刺能量")]
    private float minBoostEnergy = 15f;

    // =============== 控制系统 ===============
    [Header("CONTROL SETTINGS")]
    [SerializeField, Tooltip("鼠标灵敏度")]
    private float mouseSensitivity = 2f;

    [SerializeField, Tooltip("最大俯仰角度（度）")]
    private float maxPitchAngle = 80f;

    [SerializeField, Tooltip("旋转过渡速度")]
    private float rotationLerpSpeed = 8f;

    [SerializeField, Tooltip("机身倾斜角度限制")]
    private float bankAngleLimit = 60f;

    [SerializeField, Tooltip("机身倾斜速度")]
    private float bankingSpeed = 5f;

    [SerializeField, Tooltip("俯仰响应系数")]
    private float pitchResponseFactor = 1.2f;

    [SerializeField, Tooltip("机身模型俯仰旋转限制")]
    private float modelPitchLimit = 30f;

    [SerializeField, Tooltip("机身模型俯仰旋转速度")]
    private float modelPitchSpeed = 8f;

    [SerializeField, Tooltip("机身模型")]
    private Transform aircraftModel;

    // =============== 调试显示 ===============
    [Header("DEBUG")]
    [SerializeField, Tooltip("当前速度")]
    private float currentSpeedDisplay;

    [SerializeField, Tooltip("速度向量")]
    private Vector3 velocityDisplay;

    [SerializeField, Tooltip("当前能量")]
    private float energyDisplay;

    [SerializeField, Tooltip("能量百分比")]
    private float energyPercentDisplay;

    [SerializeField, Tooltip("刹车状态")]
    private bool isBrakingDisplay;

    [SerializeField, Tooltip("冲刺状态")]
    private bool isBoostingDisplay;

    // 私有变量
    private Rigidbody rb;
    private Vector3 currentVelocity;
    private float currentYaw;
    private float currentPitch;
    private float currentEnergy;
    private bool isBoosting;
    private bool isBraking;
    private bool isMouseLocked = true;

    // 缓存变量
    private float fixedDeltaTime;
    private Vector2 mouseDelta;
    private Vector3 movementInput;
    private Quaternion targetRot;
    private bool shiftPressed;
    private bool wPressed;
    private bool aPressed;
    private bool dPressed;
    private bool sPressed;
    private bool downArrowPressed;
    private float energyRecoveryPerFixedFrame;
    private float energyConsumptionPerFixedFrame;
    private float currentModelPitch;
    private float currentModelBank;

    // 公开属性
    public bool IsBoosting => isBoosting;
    public float CurrentSpeed => currentVelocity.magnitude;
    public float SpeedPercent => Mathf.Clamp01(currentVelocity.magnitude / maxSpeed);
    public float MaxSpeed => maxSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentEnergy = maxEnergy;

        // 预计算每固定帧的能量变化量
        energyRecoveryPerFixedFrame = energyRecoveryRate * Time.fixedDeltaTime;
        energyConsumptionPerFixedFrame = energyConsumptionRate * Time.fixedDeltaTime;
    }

    void Start()
    {
        rb.useGravity = false;
        rb.freezeRotation = true;
        InitializeMouseLock();
    }

    void Update()
    {
        fixedDeltaTime = Time.fixedDeltaTime;

        HandleMouseLock();
        UpdateInputStates();

        if (isMouseLocked)
        {
            HandleMouseRotation();
            HandleBanking();
            HandleModelPitch();

            // 每帧保持鼠标锁定状态
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        UpdateDebugInfo();
    }

    void FixedUpdate()
    {
        HandleEnergy();
        HandleMovement();
    }

#if UNITY_EDITOR
    private IEnumerator ForceMouseLockInEditor()
    {
        // 等待一帧确保编辑器完成初始化
        yield return null;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 持续三帧确保生效
        for (int i = 0; i < 3; i++)
        {
            yield return null;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
#endif

    private void InitializeMouseLock()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isMouseLocked = true;

#if UNITY_EDITOR
        StartCoroutine(ForceMouseLockInEditor());
#endif
    }

    private void HandleMouseLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isMouseLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                isMouseLocked = false;
            }
        }

        if (!isMouseLocked && Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isMouseLocked = true;
        }
    }

    private void UpdateInputStates()
    {
        shiftPressed = Input.GetKey(KeyCode.LeftShift);
        wPressed = Input.GetKey(KeyCode.W);
        aPressed = Input.GetKey(KeyCode.A);
        dPressed = Input.GetKey(KeyCode.D);
        sPressed = Input.GetKey(KeyCode.S);
        downArrowPressed = Input.GetKey(KeyCode.DownArrow);
    }

    private void HandleMouseRotation()
    {
        mouseDelta.x = Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseDelta.y = Input.GetAxis("Mouse Y") * mouseSensitivity;

        currentYaw += mouseDelta.x;
        currentPitch = Mathf.Clamp(
            currentPitch - mouseDelta.y * pitchResponseFactor,
            -maxPitchAngle,
            maxPitchAngle
        );

        targetRot = Quaternion.Euler(currentPitch, currentYaw, 0);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            rotationLerpSpeed * Time.deltaTime
        );
    }

    private void HandleBanking()
    {
        if (aircraftModel == null) return;

        float bankInput = 0f;
        if (aPressed) bankInput -= 1f;
        if (dPressed) bankInput += 1f;

        float targetBank = -bankInput * bankAngleLimit;
        currentModelBank = Mathf.LerpAngle(
            currentModelBank,
            targetBank,
            bankingSpeed * Time.deltaTime
        );
    }

    private void HandleModelPitch()
    {
        if (aircraftModel == null) return;

        float mouseY = Input.GetAxis("Mouse Y");
        float targetPitch = -mouseY * modelPitchLimit;

        currentModelPitch = Mathf.LerpAngle(
            currentModelPitch,
            targetPitch,
            modelPitchSpeed * Time.deltaTime
        );

        aircraftModel.localRotation = Quaternion.Euler(currentModelPitch, 0, currentModelBank);
    }

    private Vector3 GetMovementInput()
    {
        movementInput = Vector3.zero;

        if (wPressed) movementInput += transform.forward;
        if (dPressed) movementInput += transform.right;
        if (aPressed) movementInput -= transform.right;

        return movementInput.normalized;
    }

    private void HandleMovement()
    {
        if (isBoosting && !shiftPressed)
        {
            isBoosting = false;
        }

        isBraking = sPressed || downArrowPressed;
        Vector3 inputDir = GetMovementInput();

        if (isBraking)
        {
            ApplyDeceleration(brakeDeceleration);
            rb.velocity = currentVelocity;
            return;
        }

        if (isBoosting && currentEnergy <= 0)
        {
            isBoosting = false;
        }

        float currentAccel = isBoosting ? boostAcceleration : acceleration;
        float speedLimit = isBoosting ? maxSpeed * boostMultiplier : maxSpeed;

        if (inputDir.magnitude > 0.1f)
        {
            currentVelocity = Vector3.Lerp(
                currentVelocity,
                inputDir * speedLimit,
                currentAccel * fixedDeltaTime
            );

            currentVelocity.y = Mathf.Clamp(
                currentVelocity.y,
                -verticalSpeedLimit,
                verticalSpeedLimit
            );
        }
        else
        {
            ApplyDeceleration(naturalDeceleration);
        }

        rb.velocity = currentVelocity;
    }

    private void HandleEnergy()
    {
        if (isBoosting)
        {
            currentEnergy -= energyConsumptionPerFixedFrame;
            if (currentEnergy <= 0)
            {
                currentEnergy = 0;
                isBoosting = false;
            }
        }
        else if (shiftPressed && currentEnergy > minBoostEnergy)
        {
            isBoosting = true;
        }
        else
        {
            currentEnergy += energyRecoveryPerFixedFrame;
            if (currentEnergy > maxEnergy)
            {
                currentEnergy = maxEnergy;
            }
        }
    }

    private void ApplyDeceleration(float deceleration)
    {
        currentVelocity = Vector3.MoveTowards(
            currentVelocity,
            Vector3.zero,
            deceleration * fixedDeltaTime
        );
    }

    private void UpdateDebugInfo()
    {
        currentSpeedDisplay = currentVelocity.magnitude;
        velocityDisplay = currentVelocity;
        energyDisplay = currentEnergy;
        energyPercentDisplay = Mathf.Round(currentEnergy / maxEnergy * 100f);
        isBrakingDisplay = isBraking;
        isBoostingDisplay = isBoosting;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, currentVelocity);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 5f);

        if (isBraking)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, -currentVelocity.normalized * 3f);
        }
    }
}