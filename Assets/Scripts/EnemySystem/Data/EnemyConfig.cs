using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Base Config")]
public class EnemyConfig : ScriptableObject
{
    [Header("�ƶ�����")]
    public float maxSpeed = 10f;
    public float acceleration = 5f;
    public float rotationSpeed = 120f;
    public float stopDistance = 3f;

    [Header("�������")]
    public float drag = 1f;
    public float angularDrag = 2f;
    public ForceMode forceMode = ForceMode.Acceleration;
    public float brakingForce = 8f;

    // [!code --]
    // public float attackInterval = 2f; 
    // �������������ϵͳͳһ����
}