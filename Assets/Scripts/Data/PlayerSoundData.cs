using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSoundData", menuName = "ScriptableObjects/PlayerSoundData", order = 1)]
public class PlayerSoundData : ScriptableObject
{
    [TabGroup("Movement")]
    public AK.Wwise.Event SFX_Hunter_Movement_AirSpeed;
    [TabGroup("Movement")]
    public AK.Wwise.Event SFX_Hunter_Movement_Cloth;
    [TabGroup("Movement")]
    public AK.Wwise.Event SFX_Hunter_Movement_Footsteps;
    [TabGroup("Movement")]
    public AK.Wwise.Event SFX_Hunter_Jump;

    [TabGroup("Swing")]
    public AK.Wwise.Event SFX_Hunter_Grapple_Trigger;
    [TabGroup("Swing")]
    public AK.Wwise.Event SFX_Hunter_Grapple_Release;
    [TabGroup("Swing")]
    public AK.Wwise.Event SFX_Hunter_Hook_Double_Boost;

    [TabGroup("Attack")]
    public AK.Wwise.Event SFX_Hunter_Interaction;
    [TabGroup("Attack")]
    public AK.Wwise.Event SFX_Giant_Hit_ByHunter;

    [TabGroup("Dash")]
    public AK.Wwise.Event SFX_Hunter_Dash_Trigger;
    [TabGroup("Dash")]
    public AK.Wwise.Event SFX_Hunter_Dash_Release;

    [TabGroup("Bumper")]
    public AK.Wwise.Event SFX_Hunter_Bumper_Trigger;

    [TabGroup("Other")]
    public AK.Wwise.Event SFX_Hunter_Death;
}