using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    [Header("Camera")]
    public float walkRotationSpeed;
    public float airRotationSpeed;

    [Header("Movement")]
    public float walkSpeed;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float groundDrag;
    public float charaHeight;

    [Header("Swing")]
    public float swingSpeed;
    public float swingMaxSpeed;
    public float swingAcceleration;
    public float minSwingDistance;
    public float maxSwingDistance;
    public float startCurveSpeedBoost;
    public float endCurveSpeedBoost;
    public float radiusDetectionIncreaseSpeed;
    public float minLineDistance;
    public float fovAddition;
    public float airSlowDown;

    [Header("Attack")]
    public float attackDistance;
    public float attackStopDistance;
    public float dragForce;

    [Header("Tests")]
    public bool startCurveBoost;
    public bool endCurveBoost;

}
