using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitBottom : MonoBehaviour
{
    public void OnTriggerEnter(Collision collision)
    {
        Debug.Log("collision : " + collision.transform.name);
        Player player = collision.transform.GetComponent<Player>();
        if (player != null)
        {
            player.currentTimerPitBottom += Time.deltaTime;
            if (player.currentTimerPitBottom > GPCtrl.Instance.GeneralData.pitBottomDeathTime) GPCtrl.Instance.Loose();
        }
    }

    private void OnTriggerExit(Collision collision)
    {
        Player player = collision.transform.GetComponent<Player>();
        if (player != null)
        {
            player.currentTimerPitBottom = 0;
        }
    }
}
