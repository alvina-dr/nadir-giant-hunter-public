using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    public Player Player;
    public GameObject forwardIndication;
    public bool IsDashing;

    public void Dash()
    {
        IsDashing = true;
        GPCtrl.Instance.DashPause = false;
        Time.timeScale = 1;
        Player.Rigibody.velocity = Vector3.zero;
        Player.Rigibody.useGravity = false;
        Player.Rigibody.AddForce(Player.Data.dashForce * Camera.main.transform.forward.normalized, ForceMode.Impulse);
        StartCoroutine(StopDash());
        Debug.DrawRay(Player.transform.position, Camera.main.transform.forward* 10, Color.red, 5);
        StartCoroutine(PrintSpeed());
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
