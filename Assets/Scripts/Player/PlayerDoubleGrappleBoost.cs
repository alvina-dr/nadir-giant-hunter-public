using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PlayerDoubleGrappleBoost : MonoBehaviour
{
    public Player Player;

    private void Update()
    {
        if (Player.PlayerSwingingLeft.IsSwinging && Player.PlayerSwingingRight.IsSwinging)
        {
            Boost();
        }
    }

    public void Boost()
    {
        Player.Rigibody.AddForce(Player.Data.doubleSwingBoost * Vector3.up, ForceMode.Impulse);
        Player.PlayerSwingingLeft.StopSwing();
        Player.PlayerSwingingRight.StopSwing();
        GameObject vfx = Instantiate(Player.VFXData.doubleGrappleBoost);
        vfx.transform.position = transform.position;
    }
}
