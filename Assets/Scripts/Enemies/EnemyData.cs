using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "EnemyData", menuName ="ScriptableObjects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [TabGroup("Values")]
    public float MoveSpeed;
    [TabGroup("Values")]
    public float DistanceToGround;

    [TabGroup("Sound")]
    public AK.Wwise.Event SFX_Giant_Roar_Danger;
    [TabGroup("Sound")]
    public AK.Wwise.Event SFX_Giant_Roar_Death;
    [TabGroup("Sound")]
    public AK.Wwise.Event SFX_Giant_Movement_Grab;
    [TabGroup("Sound")]
    public AK.Wwise.Event SFX_Giant_Hit_IntoAbyss;
}
