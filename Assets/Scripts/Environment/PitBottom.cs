using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitBottom : MonoBehaviour
{
    public void OnTriggerStay(Collider collision)
    {
        if (GPCtrl.Instance.Pause) return;
        if (collision.transform.parent == null) return;
        Player player = collision.transform.parent.GetComponent<Player>();
        if (player != null)
        {
            player.currentTimerPitBottom += Time.deltaTime;
            if (player.currentTimerPitBottom > GPCtrl.Instance.GeneralData.pitBottomDeathTime)
            {
                player.Rigibody.AddForce(Vector3.up * 50, ForceMode.Impulse);
                //GPCtrl.Instance.Loose();
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.transform.parent == null) return;
        Player player = collision.transform.parent.GetComponent<Player>();
        if (player != null)
        {
            player.currentTimerPitBottom = 0;
        }
    }
}
