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

    [TabGroup("Sound")]
    public AK.Wwise.Event SFX_Giant_Roar_Danger;
    [TabGroup("Sound")]
    public AK.Wwise.Event SFX_Giant_Roar_Death;
    [TabGroup("Sound")]
    public AK.Wwise.Event SFX_Giant_Movement_Grab;
    [TabGroup("Sound")]
    public AK.Wwise.Event SFX_Giant_Hit_IntoAbyss;
}
