using Cinemachine;
using DG.Tweening;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Player Player;
    private SpringJoint _springJoint;
    [SerializeField] private Rigidbody _targetRigibody;
    [HideInInspector]
    public bool IsGrappling = false;
    public TargetableSpot CurrentTargetSpot;
    public List<TargetableSpot> ClosestTargetableSpotList = new List<TargetableSpot>();
    public Vector3 TargetSpotDistance = Vector3.zero;

    public void Attack()
    {
        if (IsGrappling) return;
        if (GPCtrl.Instance.TargetableSpotList.Count == 0) return;
        float distance = 1000;
        TargetableSpot weakSpot = null;
        for (int i = 0; i < GPCtrl.Instance.TargetableSpotList.Count; i++)
        {
            float currentDistance = Vector3.Distance(transform.position, GPCtrl.Instance.TargetableSpotList[i].transform.position);
            if (currentDistance < distance)
            {
                weakSpot = GPCtrl.Instance.TargetableSpotList[i];
                distance = currentDistance;
            }
        }
        if (weakSpot == null || distance > Player.Data.attackDistance) return;
        Vector3 direction = weakSpot.transform.position - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, Player.Data.attackDistance))
        {
            if (hit.transform.gameObject != weakSpot.gameObject) return;
            GrappleWeakSpot(weakSpot);
        }       
    }

    public void GrappleWeakSpot(TargetableSpot targetableSpot)
    {
        _targetRigibody.gameObject.SetActive(true);
        _targetRigibody.transform.position = targetableSpot.transform.position;
        Player.Collider.enabled = false;
        TargetSpotDistance = transform.position - targetableSpot.transform.position;
        Player.PlayerSwingingLeft.StopSwing(false);
        Player.PlayerSwingingRight.StopSwing(false);
        IsGrappling = true;
        CurrentTargetSpot = targetableSpot;
        Player.Rigibody.useGravity = false;
        Player.Rigibody.velocity = Vector3.zero;
        Player.SoundData.SFX_Hunter_Interaction.Post(gameObject);
        if (CurrentTargetSpot.SpotCurrentType == TargetableSpot.SpotType.WeakSpot) Player.DestructionFX.SendEvent("OnPlay");
        Time.timeScale = 0.1f;
        Material postprocess = GPCtrl.Instance.GetPostProcessMaterial();
        switch (targetableSpot.SpotCurrentType) //activate input hitframe
        {
            case TargetableSpot.SpotType.WeakSpot:
                if (postprocess != null) postprocess.SetFloat("_Timefactor_Hitframe_Input_Weakspot", Time.timeScale);
                break;
            case TargetableSpot.SpotType.DashSpot:
                if (postprocess != null) postprocess.SetFloat("_Timefactor_Hitframe_Input_Dashspot", Time.timeScale);
                break;
            case TargetableSpot.SpotType.Bumper:
                if (postprocess != null) postprocess.SetFloat("_Timefactor_Hitframe_Input_Bumper", Time.timeScale);
                break;
        }
        DOVirtual.DelayedCall(.3f, () =>
        {
            Time.timeScale = 1;
            switch (targetableSpot.SpotCurrentType) //deactivate input hitframe
            {
                case TargetableSpot.SpotType.WeakSpot:
                    if (postprocess != null) postprocess.SetFloat("_Timefactor_Hitframe_Input_Weakspot", Time.timeScale);
                    break;
                case TargetableSpot.SpotType.DashSpot:
                    if (postprocess != null) postprocess.SetFloat("_Timefactor_Hitframe_Input_Dashspot", Time.timeScale);
                    break;
                case TargetableSpot.SpotType.Bumper:
                    if (postprocess != null) postprocess.SetFloat("_Timefactor_Hitframe_Input_Bumper", Time.timeScale);
                    break;
            }
            //SPRING
            _springJoint = gameObject.AddComponent<SpringJoint>();
            _springJoint.autoConfigureConnectedAnchor = false;
            _springJoint.connectedAnchor = Vector3.zero;
            _springJoint.connectedBody = _targetRigibody;
            _springJoint.spring = 30;
            _springJoint.damper = 0f;
            _springJoint.massScale = Player.Data.dragForce;

            Player.Animator.SetTrigger("Attack");
            GPCtrl.Instance.CameraThirdPerson.CameraShake.ShakeCamera(5f, .3f);
            DataHolder.Instance.RumbleManager.PulseFor(10f, 10f, .3f);

            DOVirtual.DelayedCall(.3f, () =>
            {
                if (CurrentTargetSpot != null)
                    ReachTargetableSpot();
            });
            GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(0).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = 2f;
            GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(1).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = 2f;
            GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(2).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = 2f;
        }).SetUpdate(true);
    }

    private void Update()
    {
        GPCtrl.Instance.UICtrl.AttackInputIndicator.SetupAppearance(true, false);
        GPCtrl.Instance.UICtrl.MonsterHighIndicator.SetupAppearance(false, true);
        GPCtrl.Instance.UICtrl.AttackInput.SetVisible(false);
        ClosestTargetableSpotList = GPCtrl.Instance.TargetableSpotList;
        ClosestTargetableSpotList.Sort(delegate (TargetableSpot a, TargetableSpot b)
        {
            return Vector3.Distance(this.transform.position, a.transform.position).CompareTo(Vector3.Distance(this.transform.position, b.transform.position));
        });
        if (GPCtrl.Instance.UICtrl != null)
            GPCtrl.Instance.UICtrl.AttackInputIndicator.HideIndicator();
        if (ClosestTargetableSpotList.Count > 0 ) {
            if (Vector3.Distance(transform.position, ClosestTargetableSpotList[0].transform.position) < Player.Data.weakSpotDetectionDistance)
            {
                if (!GPCtrl.Instance.DashPause)
                {
                    TargetableSpot weakSpot = ClosestTargetableSpotList[0];
                    Vector3 direction = weakSpot.transform.position - transform.position;
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, direction, out hit, Player.Data.attackDistance))
                    {
                        if (hit.transform.gameObject != weakSpot.gameObject) return;
                        GPCtrl.Instance.UICtrl.AttackInput.SetVisible(true);
                        if (GPCtrl.Instance.EnemySpawner.EnemyList.Count > 0 && weakSpot == GPCtrl.Instance.EnemySpawner.EnemyList[0].EnemyWeakSpotManagement.WeakSpotList[0])
                        {
                            GPCtrl.Instance.UICtrl.AttackInputIndicator.SetupAppearance(true, true);
                            GPCtrl.Instance.UICtrl.MonsterHighIndicator.SetupAppearance(false, false);
                        }
                        GPCtrl.Instance.UICtrl.AttackInputIndicator.ShowIndicatorAt(ClosestTargetableSpotList[0].transform.position);
                    }
                }
            }
        }
        if (IsGrappling && _springJoint != null && _targetRigibody.gameObject.activeSelf)
        {
            if (Vector3.Distance(transform.position, _targetRigibody.transform.position) < Player.Data.attackStopDistance)
            {
                ReachTargetableSpot();
            }
        }
    }

    private void LateUpdate()
    {
        if (_springJoint && IsGrappling) //Visual effect for swing line
        {
            Player.PlayerSwingingLeft.SwingRopeFX.DrawRope(Player.PlayerSwingingLeft.StartSwingLinePoint.position, CurrentTargetSpot.transform.position);
            Player.PlayerSwingingRight.SwingRopeFX.DrawRope(Player.PlayerSwingingRight.StartSwingLinePoint.position, CurrentTargetSpot.transform.position);
        }
    }

    private void ReachTargetableSpot()
    {
        GPCtrl.Instance.CameraThirdPerson.CameraShake.ShakeCamera(10f, .1f);
        Destroy(_springJoint);
        _springJoint = null;
        IsGrappling = false;
        Player.PlayerSwingingLeft.SwingRopeFX.HideRope(Player.PlayerSwingingLeft.StartSwingLinePoint.position);
        Player.PlayerSwingingRight.SwingRopeFX.HideRope(Player.PlayerSwingingRight.StartSwingLinePoint.position);
        Player.Rigibody.velocity = Vector3.zero;
        Player.Rigibody.useGravity = true;
        Time.timeScale = 0.1f;
        Material postprocess = GPCtrl.Instance.GetPostProcessMaterial();
        switch (CurrentTargetSpot.SpotCurrentType)
        {
            case TargetableSpot.SpotType.WeakSpot:
                if (postprocess != null) postprocess.SetFloat("_Timefactor_Hitframe_Attack_Weakspot", Time.timeScale);
                break;
            case TargetableSpot.SpotType.DashSpot:
                if (postprocess != null) postprocess.SetFloat("_Timefactor_Hitframe_Attack_Dashspot", Time.timeScale);
                break;
            case TargetableSpot.SpotType.Bumper:
                if (postprocess != null) postprocess.SetFloat("_Timefactor_Hitframe_Attack_Bumper", Time.timeScale);
                break;
        }
        CurrentTargetSpot.DestroyWeakSpot();
        CurrentTargetSpot = null;
        Player.Collider.enabled = true;
        _targetRigibody.gameObject.SetActive(false);
        float factor = (Player.PlayerMovement.CurrentMoveSpeed - Player.Data.swingSpeed) / (Player.Data.swingMaxSpeed - Player.Data.swingSpeed);
        DOVirtual.Float(1f, factor * Player.Data.swingCameraDistanceAddition, .3f, v =>
        {
            GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(0).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = v;
            GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(1).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = v;
            GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(2).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = v;
        }).SetUpdate(true);
    }
}