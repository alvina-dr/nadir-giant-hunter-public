using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using DG.Tweening;
public class PlayerDoubleGrappleBoost : MonoBehaviour
{
    public Player Player;
    public bool LeftSwingReleased = true;
    public bool RightSwingReleased = true;
    public bool IsDoubleGrappling = false;

    private void Update()
    {
        if (Player.PlayerSwingingLeft.IsSwinging && Player.PlayerSwingingRight.IsSwinging)
        {
            Boost();
        }
    }

    private void LateUpdate()
    {
        if (!IsDoubleGrappling) return;
        DrawGrapple(Player.PlayerSwingingLeft);
        DrawGrapple(Player.PlayerSwingingRight);
    }

    public void DrawGrapple(PlayerSwinging playerSwinging)
    {
        if (playerSwinging.SwingLineRenderer.positionCount == 2)
        {
            playerSwinging.SwingLineRenderer.SetPosition(0, playerSwinging.StartSwingLinePoint.position);
            if (playerSwinging.SwingLineRenderer.GetPosition(1) != playerSwinging.EndSwingLinePoint.position)
            {
                playerSwinging.SwingLineRenderer.SetPosition(1, Vector3.Lerp(playerSwinging.SwingLineRenderer.GetPosition(1), playerSwinging.EndSwingLinePoint.position, 0.1f));
            }
        }
    }

    public void Boost()
    {
        Player.Rigibody.AddForce(Player.Data.doubleSwingBoost * Vector3.up, ForceMode.Impulse);
        Player.PlayerSwingingLeft.StopSwing(true, true);
        Player.PlayerSwingingRight.StopSwing(true, true);
        GameObject vfx = Instantiate(Player.VFXData.doubleGrappleBoost);
        vfx.transform.position = transform.position;
        LeftSwingReleased = false;
        RightSwingReleased = false;
        Player.PlayerSwingingRight.TrySwing = false;
        Player.PlayerSwingingLeft.TrySwing = false;
        IsDoubleGrappling = true;
        DOVirtual.DelayedCall(1f, () =>
        {
            //Player.PlayerSwingingLeft.HideLineRenderer();
            //Player.PlayerSwingingRight.HideLineRenderer();
            IsDoubleGrappling = false;
        });
    }
}
