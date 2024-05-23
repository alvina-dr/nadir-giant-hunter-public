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
    [TabGroup("Camera")]
    public float swingCameraFOVAddition;
    [TabGroup("Camera")]
    public float swingCameraDistanceAddition;

    [TabGroup("Movement")]
    public float walkSpeed;
    [TabGroup("Movement")]
    public float jumpForce;
    [TabGroup("Movement")]
    public float jumpCooldown;
    //Air control value
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
    public float SwingSpeedLengthMult;

    [TabGroup("Swing")]
    public float airSlowDown;

    [TabGroup("Attack")]
    public float attackDistance;
    [TabGroup("Attack")]
    public float attackStopDistance;
    [TabGroup("Attack")]
    public float dragForce;
    [TabGroup("Attack")]
    public float weakSpotDetectionDistance;

    [TabGroup("Double swing boost")]
    public float doubleSwingBoost;
    [TabGroup("Double swing boost")]
    public float doubleSwingLineRendererDuration;

    [TabGroup("Dash")]
    public float dashForce;
    [TabGroup("Dash")]
    public float slowDownTime;
    [TabGroup("Dash")]
    public float timeBeforeAutomaticDash;
    [TabGroup("Dash")]
    public float dashTime;

    [TabGroup("Bumper")]
    public float bumpForce;

    [Header("DEBUG")]
    public bool startCurveBoost;
    public bool endCurveBoost;
    public bool magicSwinging;
}
