using Cinemachine;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Enemies;

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
    public BoxCollider Collider;

    [ShowIf("SpotCurrentType", SpotType.WeakSpot)]
    public EnemyWeakSpotManagement Enemy;

    [ShowIf("SpotCurrentType", SpotType.DashSpot)]
    public MeshRenderer Mesh;

    public void DestroyWeakSpot()
    {
        Material postprocess = GPCtrl.Instance.GetPostProcessMaterial();
        switch (SpotCurrentType)
        {
            case SpotType.WeakSpot:
                DOVirtual.DelayedCall(.2f, () =>
                {
                    if (GPCtrl.Instance.Pause) return;
                    Time.timeScale = 1;
                    if (postprocess != null) postprocess.SetFloat("_Timefactor_Hitframe_Attack_Weakspot", Time.timeScale);
                }).SetUpdate(true);
                if (Enemy != null)
                {
                    GameObject vfx = Instantiate(Enemy.VFXData.weakSpotExplosion);
                    vfx.transform.position = transform.position;
                    Enemy.Damage(this);
                }
                VisualFX.SendEvent("Destroy");
                VisualFX.SetBool("kill tentacles", true);
                //VisualFX.SetFloat("lerp from 0 to 1 during attack dash", 0);
                DOVirtual.Float(1f, 0, .3f, v =>
                {
                    VisualFX.SetFloat("lerp from 0 to 1 during attack dash", v);
                }).SetUpdate(true); 
                StartCoroutine(DestroyAfterDelay());
                GPCtrl.Instance.TargetableSpotList.Remove(this);
                Collider.enabled = false;
                Vector3 vector3 = GPCtrl.Instance.Player.PlayerAttack.TargetSpotDistance.normalized;
                Vector3 newDirection = new Vector3(-vector3.x, vector3.y, -vector3.z) * GPCtrl.Instance.Player.Data.weakSpotReboundForce;
                GPCtrl.Instance.Player.Rigibody.velocity = Vector3.zero;
                GPCtrl.Instance.Player.Rigibody.AddForce(newDirection, ForceMode.Impulse);
                GPCtrl.Instance.Player.SoundData.SFX_Giant_Hit_ByHunter.Post(gameObject);
                break;
            case SpotType.DashSpot:
                GPCtrl.Instance.DashPause = true;
                Time.timeScale = GPCtrl.Instance.Player.Data.slowDownTime;
                if (postprocess != null) postprocess.SetFloat("_Timefactor_Hitframe_Attack_Dashspot", 1); //end of attack hitframe
                if (postprocess != null) postprocess.SetFloat("_Timefactor_Dashspot_Timestop", Time.timeScale); //start dash slow mo
                StartCoroutine(DashSlowDown());
                StartCoroutine(ReloadDashSpot());
                GPCtrl.Instance.Player.PlayerDash.CurrentDashSpot = this;
                VisualFX.SendEvent("collision");
                GPCtrl.Instance.TargetableSpotList.Remove(this);
                Material material = GPCtrl.Instance.GetPostProcessMaterial();
                material.DOFloat(1f, "_strength", .2f).SetUpdate(true);
                VisualFX.SetFloat("effect strength", 0);
                GPCtrl.Instance.Player.SoundData.SFX_Hunter_Dash_Trigger.Post(GPCtrl.Instance.Player.gameObject);
                AkSoundEngine.SetState("SlowMo", "InSlowMo");
                break;
            case SpotType.Bumper:
                Time.timeScale = 1;

                DOVirtual.DelayedCall(.1f, () =>
                {
                    if (GPCtrl.Instance.Pause) return;
                    Time.timeScale = 1;
                    if (postprocess != null) postprocess.SetFloat("_Timefactor_Hitframe_Attack_Bumper", Time.timeScale);
                }).SetUpdate(true);
                Bump();
                StartCoroutine(ReloadBumper());
                GPCtrl.Instance.Player.SoundData.SFX_Hunter_Bumper_Trigger.Post(gameObject);
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
        yield return new WaitForSeconds(GPCtrl.Instance.GeneralData.dashSpotReloadTime);
        GPCtrl.Instance.TargetableSpotList.Add(this);
        VisualFX.SetFloat("effect strength", 1f);
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
        if (GPCtrl.Instance.Player.PlayerSwingingLeft.EndSwingLinePoint.parent == transform)
            GPCtrl.Instance.Player.PlayerSwingingLeft.EndSwingLinePoint.parent = null;
        if (GPCtrl.Instance.Player.PlayerSwingingRight.EndSwingLinePoint.parent == transform)
            GPCtrl.Instance.Player.PlayerSwingingRight.EndSwingLinePoint.parent = null;
        Destroy(gameObject);
    }
    
    public void Bump()
    {
        GPCtrl.Instance.Player.Rigibody.velocity = Vector3.zero;
        GPCtrl.Instance.Player.Rigibody.AddForce(transform.up * GPCtrl.Instance.Player.Data.bumpForce, ForceMode.Impulse);
        VisualFX.SendEvent("trigger2");
    }
}