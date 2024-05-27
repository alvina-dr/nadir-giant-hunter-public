using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GeneralData", menuName = "ScriptableObjects/GeneralData", order = 1)]
public class GeneralData : ScriptableObject
{
    public float levelMaxTime;
    public float pitBottomDeathTime;
    public float dashSpotReloadTime;
    public float yHeightGameOver;
    public int scoreboardSize;
}
