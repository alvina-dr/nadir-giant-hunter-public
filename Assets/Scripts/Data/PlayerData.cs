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
    [TabGroup("Movement")]
    public float timeBeforeLookingDownAnim;
    [TabGroup("Movement")]
    public float speedDivisionFactorVFX;
    [TabGroup("Movement")]
    public float maxSpeedInAir;

    [TabGroup("Swing")]
    public float swingSpeed;
    [TabGroup("Swing")]
    public float swingMaxSpeed;
    [TabGroup("Swing")]
    public float SwingSpeedLengthMult;
    [TabGroup("Swing"), Tooltip("How much base direction direction is kept during swing")]
    public float SwingBaseOrientationSpeed;
    [TabGroup("Swing"), Tooltip("How much looking right and left will affect direction during swing")]
    public float SwingCameraOrientInfluence;
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
    [TabGroup("Swing"), Tooltip("when is the swing stopped automatically, between 0 and 1")]
    public float MaxSwingAngle;
    [TabGroup("Swing")]
    public float maxYForceOnRelease;

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
    [TabGroup("Attack")]
    public float weakSpotReboundForce;

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
