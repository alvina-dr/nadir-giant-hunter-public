using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "EnemyData", menuName ="ScriptableObjects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [TabGroup("Movement")]
    public float DistanceToGround;
    [TabGroup("Movement")]
    public float InitialSpeed;
    [TabGroup("Movement")]
    public float FinalSpeed;
    [TabGroup("Movement")]
    public float FinalSpeedHeight;
}