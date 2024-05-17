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
        GPCtrl.Instance.reliefFX.enabled = false;
        Player.Meshtrail.ShowTrail();
    }

    private IEnumerator PrintSpeed()
    {
        yield return new WaitForFixedUpdate();
    }

    private IEnumerator StopDash()
    {
        yield return new WaitForSecondsRealtime(Player.Data.dashTime);
        Player.Rigibody.useGravity = true;
        IsDashing = false;
    }
}
