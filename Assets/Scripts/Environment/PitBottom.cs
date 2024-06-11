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
            Material postProcess = GPCtrl.Instance.GetPostProcessMaterial();
            if (postProcess != null)
            {
                postProcess.SetFloat("_Hit_by_Abyss_Time", Mathf.Min(player.currentTimerPitBottom / GPCtrl.Instance.GeneralData.pitBottomDeathTime, 1.0f));
            }
            if (player.currentTimerPitBottom > GPCtrl.Instance.GeneralData.pitBottomDeathTime)
            {
                if (GPCtrl.Instance.GeneralData.debugBouncingGround)
                    player.Rigibody.AddForce(Vector3.up * 200, ForceMode.Impulse);
                else 
                    GPCtrl.Instance.Loose();
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
            Material postProcess = GPCtrl.Instance.GetPostProcessMaterial();
            if (postProcess != null)
            {
                postProcess.SetFloat("_Hit_by_Abyss_Time", 0);
            }
        }
    }
}
