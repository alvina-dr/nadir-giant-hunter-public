using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySoundData", menuName = "ScriptableObjects/EnemySoundData")]
public class EnemySoundData : ScriptableObject
{
    [TabGroup("Sound")]
    public AK.Wwise.Event SFX_Giant_Roar_Danger;
    [TabGroup("Sound")]
    public AK.Wwise.Event SFX_Giant_Roar_Death;
    [TabGroup("Sound")]
    public AK.Wwise.Event SFX_Giant_Movement_Grab;
    [TabGroup("Sound")]
    public AK.Wwise.Event SFX_Giant_Hit_IntoAbyss;
}
