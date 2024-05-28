using DG.Tweening;
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
                DOVirtual.DelayedCall(.2f, () =>
                {
                    Time.timeScale = 1;
                }).SetUpdate(true);
                if (Enemy != null)
                {
                    GameObject vfx = Instantiate(Enemy.VFXData.weakSpotExplosion);
                    vfx.transform.position = transform.position;
                    Enemy.Damage(this);
                }
                VisualFX.SendEvent("Destroy");
                VisualFX.SetBool("kill tentacles", true);
                StartCoroutine(DestroyAfterDelay());
                GPCtrl.Instance.TargetableSpotList.Remove(this);
                Vector3 vector3 = GPCtrl.Instance.Player.PlayerAttack.TargetSpotDistance.normalized;
                Vector3 newDirection = new Vector3(-vector3.x, vector3.y, -vector3.z) * GPCtrl.Instance.Player.Data.weakSpotReboundForce;
                GPCtrl.Instance.Player.Rigibody.velocity = Vector3.zero;
                GPCtrl.Instance.Player.Rigibody.AddForce(newDirection, ForceMode.Impulse);
                break;
            case SpotType.DashSpot:
                GPCtrl.Instance.DashPause = true;
                Time.timeScale = GPCtrl.Instance.Player.Data.slowDownTime;
                StartCoroutine(DashSlowDown());
                StartCoroutine(ReloadDashSpot());
                GPCtrl.Instance.Player.PlayerDash.CurrentDashSpot = this;
                VisualFX.SendEvent("collision");
                GPCtrl.Instance.TargetableSpotList.Remove(this);
                Material material = GPCtrl.Instance.GetPostProcessMaterial();
                material.DOFloat(1f, "_strength", .2f).SetUpdate(true);
                break;
            case SpotType.Bumper:
                DOVirtual.DelayedCall(.1f, () =>
                {
                    Time.timeScale = 1;
                }).SetUpdate(true);
                Bump();
                StartCoroutine(ReloadBumper());
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
        VisualFX.SendEvent("OnPlay");
    }

    private IEnumerator ReloadBumper()
    {
        yield return new WaitForSecondsRealtime(1);
        GPCtrl.Instance.TargetableSpotList.Add(this);
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSecondsRealtime(5.0f);
        Destroy(gameObject);
    }
    
    public void Bump()
    {
        GPCtrl.Instance.Player.Rigibody.velocity = Vector3.zero;
        GPCtrl.Instance.Player.Rigibody.AddForce(transform.up * GPCtrl.Instance.Player.Data.bumpForce, ForceMode.Impulse);
        VisualFX.SendEvent("trigger2");
    }
}