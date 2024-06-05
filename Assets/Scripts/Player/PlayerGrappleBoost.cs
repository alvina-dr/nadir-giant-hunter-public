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
        Player.PlayerSwingingLeft.SwingRopeFX.DrawRope(Player.PlayerSwingingLeft.StartSwingLinePoint.position, Player.PlayerSwingingLeft.EndSwingLinePoint.position);
        Player.PlayerSwingingRight.SwingRopeFX.DrawRope(Player.PlayerSwingingRight.StartSwingLinePoint.position, Player.PlayerSwingingRight.EndSwingLinePoint.position);
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
        GPCtrl.Instance.CameraThirdPerson.CameraShake.ShakeCamera(5f, .3f);
        DOVirtual.DelayedCall(Player.Data.doubleSwingLineRendererDuration, () =>
        {
            Player.PlayerSwingingLeft.SwingRopeFX.HideRope(Player.PlayerSwingingLeft.StartSwingLinePoint.position);
            Player.PlayerSwingingRight.SwingRopeFX.HideRope(Player.PlayerSwingingRight.StartSwingLinePoint.position);
            IsGrapplingBoost = false;
        }).OnUpdate(() =>
        {
            if (Player.PlayerSwingingLeft.EndSwingLinePoint.transform.position.y < Player.transform.position.y)
                Player.PlayerSwingingLeft.SwingRopeFX.HideRope(Player.PlayerSwingingLeft.StartSwingLinePoint.position);
            if (Player.PlayerSwingingRight.EndSwingLinePoint.transform.position.y < Player.transform.position.y)
                Player.PlayerSwingingRight.SwingRopeFX.HideRope(Player.PlayerSwingingRight.StartSwingLinePoint.position);
        });
    }
}
