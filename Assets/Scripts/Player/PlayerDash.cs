using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    public Player Player;
    public bool IsDashing;
    public TargetableSpot CurrentDashSpot;

    public void Dash()
    {
        if (CurrentDashSpot != null)
        {
            CurrentDashSpot.VisualFX.SendEvent("stop");
            CurrentDashSpot = null;
        }
        IsDashing = true;
        GPCtrl.Instance.DashPause = false;
        Time.timeScale = 1;
        Player.Rigibody.velocity = Vector3.zero;
        Player.Rigibody.useGravity = false;
        Player.Rigibody.AddForce(Player.Data.dashForce * Camera.main.transform.forward.normalized, ForceMode.Impulse);
        StartCoroutine(StopDash());
        StartCoroutine(PrintSpeed());
        Material material = GPCtrl.Instance.GetPostProcessMaterial();
        material.DOFloat(0f, "_strength", .2f).SetUpdate(true);
        Player.Meshtrail.ShowTrail();
        GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(0).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = 1f;
        GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(1).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = 1f;
        GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(2).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = 1f;
        GPCtrl.Instance.CameraThirdPerson.CameraShake.ShakeCamera(5f, .3f);
    }

    private IEnumerator PrintSpeed()
    {
        yield return new WaitForFixedUpdate();
    }

    private IEnumerator StopDash()
    {
        yield return new WaitForSecondsRealtime(Player.Data.dashTime);
        Player.Rigibody.useGravity = true;
        float factor = (Player.PlayerMovement.CurrentMoveSpeed - Player.Data.swingSpeed) / (Player.Data.swingMaxSpeed - Player.Data.swingSpeed);
        DOVirtual.Float(1f, factor * Player.Data.swingCameraDistanceAddition, .3f, v =>
        {
            Debug.Log("float : " + v);
            GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(0).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = v;
            GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(1).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = v;
            GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(2).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = v;
        }).OnComplete(() =>
        {
            IsDashing = false;
        });

    }
}
