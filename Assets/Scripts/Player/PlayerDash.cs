using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    public Player Player;
    public void Dash()
    {
        Debug.Log("Dash");
        GPCtrl.Instance.DashPause = false;
        Time.timeScale = 1;
        Player.Rigibody.velocity = Vector3.zero;
        Player.Rigibody.AddForce(Player.Data.dashForce * Camera.main.transform.forward, ForceMode.Impulse);
        Player.Rigibody.useGravity = false;
        StartCoroutine(StopDash());
    }

    private IEnumerator StopDash()
    {
        yield return new WaitForSeconds(Player.Data.dashTime);
        Player.Rigibody.useGravity = true;
    }
}
