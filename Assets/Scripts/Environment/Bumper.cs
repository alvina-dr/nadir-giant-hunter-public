using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent == null) return;
        Player player = other.transform.parent.GetComponent<Player>();
        if (player != null)
        {
            player.Rigibody.AddForce(Vector3.up * player.Data.bumpForce, ForceMode.Impulse);
        }
    }
}
