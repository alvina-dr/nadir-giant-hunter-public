using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetableSpot : MonoBehaviour
{
    public Rigidbody Rigidbody;
    public EnemyWeakSpotManagement Enemy;
    public enum SpotType
    {
        WeakSpot = 0,
        DashSpot = 1
    }
    public SpotType SpotCurrentType;

    public void DestroyWeakSpot()
    {
        switch (SpotCurrentType)
        {
            case SpotType.WeakSpot:
                if (Enemy != null)
                {
                    GameObject vfx = Instantiate(Enemy.VFXData.weakSpotExplosion);
                    vfx.transform.position = transform.position;
                    Enemy.Damage(this);
                }
                break;
            case SpotType.DashSpot:
                GPCtrl.Instance.DashPause = true;
                Time.timeScale = GPCtrl.Instance.Player.Data.slowDownTime;
                StartCoroutine(DashSlowDown());
                break;
        }
        GPCtrl.Instance.TargetableSpotList.Remove(this);
        Destroy(gameObject);
    }

    private IEnumerator DashSlowDown()
    {
        yield return new WaitForSeconds(GPCtrl.Instance.Player.Data.timeBeforeAutomaticDash);
        if (GPCtrl.Instance.DashPause) GPCtrl.Instance.Player.PlayerDash.Dash();
    }
}