using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakSpot : MonoBehaviour
{
    public Rigidbody Rigidbody;
    public EnemyVFXData VFXData;
    public EnemyWeakSpotManagement Enemy;
    public void DestroyWeakSpot()
    {
        //add particles and destroy sound here + destroy titan
        GameObject vfx = Instantiate(VFXData.weakSpotExplosion);
        vfx.transform.position = transform.position;
        if (Enemy != null) Enemy.Damage(this);
        GPCtrl.Instance.WeakSpotList.Remove(this);
        Destroy(gameObject);
    }
}
