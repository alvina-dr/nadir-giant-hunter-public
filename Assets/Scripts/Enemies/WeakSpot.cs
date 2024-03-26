using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakSpot : MonoBehaviour
{
    public Rigidbody Rigidbody;

    public void DestroyWeakSpot()
    {
        //add particles and destroy sound here + destroy titan
        GPCtrl.Instance.WeakSpotList.Remove(this);
        Destroy(gameObject);
    }
}
