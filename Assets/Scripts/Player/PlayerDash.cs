using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    public Player Player;
    public GameObject forwardIndication;
    public bool IsDashing;

    private void Update()
    {
        //if (GPCtrl.Instance.DashPause)
        //{
            //forwardIndication.transform.forward = (GPCtrl.Instance.Player.transform.position - Camera.main.transform.position).normalized;
        //}
    }

    public void Dash()
    {
        IsDashing = true;
        Debug.Log("Dash : " + Camera.main.transform.forward.normalized);
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
        Debug.Log("rb velocity : " + Player.Rigibody.velocity);
    }

    private IEnumerator StopDash()
    {
        yield return new WaitForSecondsRealtime(Player.Data.dashTime);
        Debug.Log("STOP DASH");
        Player.Rigibody.useGravity = true;
        IsDashing = false;
    }
}
