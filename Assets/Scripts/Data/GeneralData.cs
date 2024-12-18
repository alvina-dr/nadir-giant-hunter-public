using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GeneralData", menuName = "ScriptableObjects/GeneralData", order = 1)]
public class GeneralData : ScriptableObject
{
    public float levelMaxTime;
    public float pitBottomDeathTime;
    public float dashSpotReloadTime;
    public int scoreboardSize;

    [Header("Pit height")]
    public float yHeightGameOver;
    public float yHeightPitWarning;
    public float yHeightPitBottom;

    [Header("Optimisation")]
    public int ConeRaycastFrameUpdate;

    [Header("DEBUG")]
    public bool debugBouncingGround;
}