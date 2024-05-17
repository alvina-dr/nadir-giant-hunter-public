using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Bumper : MonoBehaviour
{
    public VisualEffect VisualFX;
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent == null) return;
        Player player = other.transform.parent.GetComponent<Player>();
        if (player != null)
        {
            player.Rigibody.AddForce(transform.up * player.Data.bumpForce, ForceMode.Impulse);
            VisualFX.SendEvent("trigger2");
        }
    }
}
