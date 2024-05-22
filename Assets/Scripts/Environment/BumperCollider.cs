using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BumperCollider : MonoBehaviour
{
    [SerializeField] private TargetableSpot _targetableSpot;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent == null) return;
        Player player = other.transform.parent.GetComponent<Player>();
        if (player != null)
        {
            _targetableSpot.Bump();
        }
    }
}
