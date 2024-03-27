using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using DG.Tweening;
public class PlayerDoubleGrappleBoost : MonoBehaviour
{
    public Player Player;
    public bool leftSwingReleased = true;
    public bool rightSwingReleased = true;

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
        Player.PlayerSwingingLeft.StopSwing(true, false);
        Player.PlayerSwingingRight.StopSwing(true, false);
        GameObject vfx = Instantiate(Player.VFXData.doubleGrappleBoost);
        vfx.transform.position = transform.position;
        leftSwingReleased = false;
        rightSwingReleased = false;
        Player.PlayerSwingingRight.TrySwing = false;
        Player.PlayerSwingingLeft.TrySwing = false;
        DOVirtual.DelayedCall(1f, () =>
        {
            Player.PlayerSwingingLeft.HideLineRenderer();
            Player.PlayerSwingingRight.HideLineRenderer();
        });
    }
}
