using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{

    public Player Player;

    public void Step()
    {
        Player.SoundData.SFX_Hunter_Movement_Footsteps.Post(gameObject);
    }
}
