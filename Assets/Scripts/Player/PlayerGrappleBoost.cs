using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using DG.Tweening;
public class PlayerGrappleBoost : MonoBehaviour
{
    public Player Player;
    public bool LeftSwingReleased = true;
    public bool RightSwingReleased = true;
    public bool IsGrapplingBoost = false;

    private void LateUpdate()
    {
        if (!IsGrapplingBoost) return;
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
        Player.SoundData.SFX_Hunter_Hook_Double_Boost.Post(gameObject);
        Vector3 boostDirection = Vector3.zero;
        if (Player.PlayerSwingingLeft.IsSwinging)
            boostDirection += Player.PlayerSwingingLeft.EndSwingLinePoint.position - Player.PlayerSwingingLeft.StartSwingLinePoint.position;
        if (Player.PlayerSwingingRight.IsSwinging)
            boostDirection += Player.PlayerSwingingRight.EndSwingLinePoint.position - Player.PlayerSwingingRight.StartSwingLinePoint.position;
        if (boostDirection == Vector3.zero) boostDirection = Vector3.up;
        boostDirection = boostDirection.normalized;
        Player.Rigibody.AddForce(Player.Data.doubleSwingBoost * boostDirection, ForceMode.Impulse);
        Player.PlayerSwingingLeft.StopSwing(true, false);
        Player.PlayerSwingingRight.StopSwing(true, false);
        GameObject vfx = Instantiate(Player.VFXData.doubleGrappleBoost);
        vfx.transform.position = transform.position;
        Player.PlayerSwingingRight.IsTrySwing = false;
        Player.PlayerSwingingLeft.IsTrySwing = false;
        IsGrapplingBoost = true;
        DOVirtual.DelayedCall(Player.Data.doubleSwingLineRendererDuration, () =>
        {
            Player.PlayerSwingingLeft.HideLineRenderer();
            Player.PlayerSwingingRight.HideLineRenderer();
            IsGrapplingBoost = false;
        }).OnUpdate(() =>
        {
            if (Player.PlayerSwingingLeft.EndSwingLinePoint.transform.position.y < Player.transform.position.y)
                Player.PlayerSwingingLeft.HideLineRenderer();
            if (Player.PlayerSwingingRight.EndSwingLinePoint.transform.position.y < Player.transform.position.y)
                Player.PlayerSwingingRight.HideLineRenderer();
        });
    }
}
