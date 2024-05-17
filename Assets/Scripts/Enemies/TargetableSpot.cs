using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TargetableSpot : MonoBehaviour
{
    public Rigidbody Rigidbody;
    public enum SpotType
    {
        WeakSpot = 0,
        DashSpot = 1, 
        Bumper = 2
    }
    public SpotType SpotCurrentType;
    public VisualEffect VisualFX;
    [ShowIf("SpotCurrentType", SpotType.WeakSpot)]
    public EnemyWeakSpotManagement Enemy;

    [ShowIf("SpotCurrentType", SpotType.DashSpot)]
    public MeshRenderer Mesh;

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
                VisualFX.SendEvent("collision");
                StartCoroutine(DestroyAfterDelay());
                GPCtrl.Instance.TargetableSpotList.Remove(this);
                break;
            case SpotType.DashSpot:
                GPCtrl.Instance.DashPause = true;
                Time.timeScale = GPCtrl.Instance.Player.Data.slowDownTime;
                //GPCtrl.Instance.reliefFX.customPasses.
                GPCtrl.Instance.reliefFX.enabled = true;
                StartCoroutine(DashSlowDown());
                StartCoroutine(ReloadDashSpot());
                GPCtrl.Instance.Player.PlayerDash.CurrentDashSpot = this;
                VisualFX.SendEvent("collision");
                //Mesh.enabled = false;
                GPCtrl.Instance.TargetableSpotList.Remove(this);
                break;
        }

    }

    private IEnumerator DashSlowDown()
    {
        yield return new WaitForSecondsRealtime(GPCtrl.Instance.Player.Data.timeBeforeAutomaticDash);
        if (GPCtrl.Instance.DashPause) GPCtrl.Instance.Player.PlayerDash.Dash();
    }

    private IEnumerator ReloadDashSpot()
    {
        yield return new WaitForSecondsRealtime(GPCtrl.Instance.GeneralData.dashSpotReloadTime);
        GPCtrl.Instance.TargetableSpotList.Add(this);
        //Mesh.enabled = true;
        VisualFX.SendEvent("OnPlay");
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        Destroy(gameObject);
    }
}