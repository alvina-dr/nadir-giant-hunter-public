using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    [TabGroup("Camera")]
    public float walkRotationSpeed;
    [TabGroup("Camera")]
    public float airRotationSpeed;

    [TabGroup("Movement")]
    public float walkSpeed;
    [TabGroup("Movement")]
    public float jumpForce;
    [TabGroup("Movement")]
    public float jumpCooldown;
    [TabGroup("Movement")]
    public float airMultiplier;
    [TabGroup("Movement")]
    public float groundDrag;
    [TabGroup("Movement")]
    public float charaHeight;

    [TabGroup("Swing")]
    public float swingSpeed;
    [TabGroup("Swing")]
    public float swingMaxSpeed;
    [TabGroup("Swing")]
    public float swingAcceleration;
    [TabGroup("Swing")]
    public float minSwingDistance;
    [TabGroup("Swing")]
    public float maxSwingDistance;
    [TabGroup("Swing")]
    public float startCurveSpeedBoost;
    [TabGroup("Swing")]
    public float endCurveSpeedBoost;
    [TabGroup("Swing")]
    public float radiusDetectionIncreaseSpeed;

    [TabGroup("Swing")]
    public float fovAddition;
    [TabGroup("Swing")]
    public float airSlowDown;

    [TabGroup("Attack")]
    public float attackDistance;
    [TabGroup("Attack")]
    public float attackStopDistance;
    [TabGroup("Attack")]
    public float dragForce;

    [TabGroup("Double swing boost")]
    public float doubleSwingBoost;
    [TabGroup("Double swing boost")]
    public float doubleSwingLineRendererDuration;

    [Header("Tests")]
    public bool startCurveBoost;
    public bool endCurveBoost;
}
