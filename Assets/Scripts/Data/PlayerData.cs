using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    [Header("Movement")]
    public float walkSpeed;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float groundDrag;

    [Header("Swing")]
    public float swingSpeed;
    public float swingMaxSpeed;
    public float swingAcceleration;
    public float maxSwingDistance;
    public float endCurveSpeedBoost;
    public float radiusDetectionIncreaseSpeed;
}
