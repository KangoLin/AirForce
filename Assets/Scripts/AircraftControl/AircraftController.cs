using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AircraftController : MonoBehaviour
{
    // =============== �����˶����� ===============
    [Header("MOVEMENT SETTINGS")]
    [SerializeField, Tooltip("�������ٶȣ���/��?��")]
    private float acceleration = 25f;

    [SerializeField, Tooltip("����ƶ��ٶȣ���/�룩")]
    private float maxSpeed = 100f;

    [SerializeField, Tooltip("��ֱ�ٶ����ƣ���/�룩")]
    private float verticalSpeedLimit = 50f;

    [SerializeField, Tooltip("��Ȼ���ٶȣ���/��?��")]
    private float naturalDeceleration = 5f;

    [SerializeField, Tooltip("ɲ�����ٶȣ���/��?��")]
    private float brakeDeceleration = 20f;

    // =============== ���ϵͳ ===============
    [Header("BOOST SYSTEM")]
    [SerializeField, Tooltip("����ٶȱ���")]
    private float boostMultiplier = 1.5f;

    [SerializeField, Tooltip("��̼��ٶ�")]
    private float boostAcceleration = 50f;

    // =============== ����ϵͳ ===============
    [Header("ENERGY SYSTEM")]
    [SerializeField, Tooltip("�������ֵ")]
    private float maxEnergy = 100f;

    [SerializeField, Tooltip("�����������/��")]
    private float energyConsumptionRate = 5f;

    [SerializeField, Tooltip("�����ָ�/��")]
    private float energyRecoveryRate = 3f;

    [SerializeField, Tooltip("��ͳ������")]
    private float minBoostEnergy = 15f;

    // =============== ����ϵͳ ===============
    [Header("CONTROL SETTINGS")]
    [SerializeField, Tooltip("���������")]
    private float mouseSensitivity = 2f;

    [SerializeField, Tooltip("������Ƕȣ��ȣ�")]
    private float maxPitchAngle = 80f;

    [SerializeField, Tooltip("��ת�����ٶ�")]
    private float rotationLerpSpeed = 8f;

    [SerializeField, Tooltip("������б�Ƕ�����")]
    private float bankAngleLimit = 60f;

    [SerializeField, Tooltip("������б�ٶ�")]
    private float bankingSpeed = 5f;

    [SerializeField, Tooltip("������Ӧϵ��")]
    private float pitchResponseFactor = 1.2f;

    [SerializeField, Tooltip("����ģ�͸�����ת����")]
    private float modelPitchLimit = 30f;

    [SerializeField, Tooltip("����ģ�͸�����ת�ٶ�")]
    private float modelPitchSpeed = 8f;

    [SerializeField, Tooltip("����ģ��")]
    private Transform aircraftModel;

    // =============== ������ʾ ===============
    [Header("DEBUG")]
    [SerializeField, Tooltip("��ǰ�ٶ�")]
    private float currentSpeedDisplay;

    [SerializeField, Tooltip("�ٶ�����")]
    private Vector3 velocityDisplay;

    [SerializeField, Tooltip("��ǰ����")]
    private float energyDisplay;

    [SerializeField, Tooltip("�����ٷֱ�")]
    private float energyPercentDisplay;

    [SerializeField, Tooltip("ɲ��״̬")]
    private bool isBrakingDisplay;

    [SerializeField, Tooltip("���״̬")]
    private bool isBoostingDisplay;

    // ˽�б���
    private Rigidbody rb;
    private Vector3 currentVelocity;
    private float currentYaw;
    private float currentPitch;
    private float currentEnergy;
    private bool isBoosting;
    private bool isBraking;
    private bool isMouseLocked = true;

    // �������
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

    // ��������
    public bool IsBoosting => isBoosting;
    public float CurrentSpeed => currentVelocity.magnitude;
    public float SpeedPercent => Mathf.Clamp01(currentVelocity.magnitude / maxSpeed);
    public float MaxSpeed => maxSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentEnergy = maxEnergy;

        // Ԥ����ÿ�̶�֡�������仯��
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

            // ÿ֡�����������״̬
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
        // �ȴ�һ֡ȷ���༭����ɳ�ʼ��
        yield return null;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // ������֡ȷ����Ч
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