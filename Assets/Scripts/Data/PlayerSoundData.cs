using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSoundData", menuName = "ScriptableObjects/PlayerSoundData", order = 1)]
public class PlayerSoundData : ScriptableObject
{

    [TabGroup("Hook")]
    public AK.Wwise.Event SFX_Hunter_Hook_Single_Trigger;
    [TabGroup("Hook")]
    public AK.Wwise.Event SFX_Hunter_Hook_Single_Release;
    [TabGroup("Hook")]
    public AK.Wwise.Event SFX_Hunter_Hook_Single_Grappled;
    [TabGroup("Hook")]
    public AK.Wwise.Event SFX_Hunter_Hook_Double_Trigger;
    [TabGroup("Hook")]
    public AK.Wwise.Event SFX_Hunter_Hook_Double_Release;
    [TabGroup("Hook")]
    public AK.Wwise.Event SFX_Hunter_Hook_Double_Grappled;
    [TabGroup("Hook")]
    public AK.Wwise.Event SFX_Hunter_Hook_Double_Boost;

    [TabGroup("Attack")]
    public AK.Wwise.Event SFX_Hunter_Attack_Rush;
    [TabGroup("Attack")]
    public AK.Wwise.Event SFX_Hunter_Attack_Impact;

    [TabGroup("Movement")]
    public AK.Wwise.Event SFX_Hunter_Movement_AirSpeed;
    [TabGroup("Movement")]
    public AK.Wwise.Event SFX_Hunter_Movement_Cloth;
    [TabGroup("Movement")]
    public AK.Wwise.Event SFX_Hunter_Movement_Footsteps;
    [TabGroup("Movement")]
    public AK.Wwise.Event SFX_Hunter_Jump;

}
