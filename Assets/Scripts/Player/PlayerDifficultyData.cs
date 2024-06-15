using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDifficultyData", menuName = "ScriptableObjects/PlayerDifficultyData", order = 1)]
public class PlayerDifficultyData : ScriptableObject
{
    public float TargetableSpotDetectionDistance;
    public float DashForce;
}