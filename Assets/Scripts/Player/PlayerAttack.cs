using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Player Player;
    private SpringJoint _springJoint;
    [HideInInspector]
    public bool IsGrappling = false;
    public TargetableSpot CurrentTargetSpot;
    public List<TargetableSpot> closestTargetableSpotList = new List<TargetableSpot>();
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

    public void GrappleWeakSpot(TargetableSpot weakSpot)
    {
        Player.Collider.enabled = false;
        TargetSpotDistance = transform.position - weakSpot.transform.position;
        Player.PlayerSwingingLeft.StopSwing(false);
        Player.PlayerSwingingRight.StopSwing(false);
        IsGrappling = true;
        CurrentTargetSpot = weakSpot;
        Player.Rigibody.useGravity = false;
        Player.Rigibody.velocity = Vector3.zero;

        //SPRING
        _springJoint = gameObject.AddComponent<SpringJoint>();
        _springJoint.autoConfigureConnectedAnchor = false;
        _springJoint.connectedAnchor = Vector3.zero;
        _springJoint.connectedBody = weakSpot.Rigidbody;
        _springJoint.spring = 30;
        _springJoint.damper = 0f;
        _springJoint.massScale = Player.Data.dragForce;

        Player.Animator.SetTrigger("Attack");

        Player.SoundData.SFX_Hunter_Attack_Rush.Post(gameObject);

        GPCtrl.Instance.CameraThirdPerson.CameraShake.ShakeCamera(5f, .3f);
        //GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(0).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = 1f;
        //GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(1).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = 1f;
        //GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(2).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = 1f;
        //GPCtrl.Instance.CameraThirdPerson.CameraShake.ShakeCamera(5f, .3f);
        //float factor = (Player.PlayerMovement.CurrentMoveSpeed - Player.Data.swingSpeed) / (Player.Data.swingMaxSpeed - Player.Data.swingSpeed);
        //DOVirtual.Float(1f, factor * Player.Data.swingCameraDistanceAddition, Player.Data.dashTime, v =>
        //{
        //    //Debug.Log("float : " + v);
        //    GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(0).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = v;
        //    GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(1).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = v;
        //    GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(2).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = v;
        //});
    }

    private void Update()
    {
        closestTargetableSpotList = GPCtrl.Instance.TargetableSpotList;
        closestTargetableSpotList.Sort(delegate (TargetableSpot a, TargetableSpot b)
        {
            return Vector3.Distance(this.transform.position, a.transform.position).CompareTo(Vector3.Distance(this.transform.position, b.transform.position));
        });
        if (GPCtrl.Instance.UICtrl != null)
            GPCtrl.Instance.UICtrl.AttackInputIndicator.HideIndicator();
        if (closestTargetableSpotList.Count > 0 ) {
            if (Vector3.Distance(transform.position, closestTargetableSpotList[0].transform.position) < Player.Data.weakSpotDetectionDistance)
            {
                if (!GPCtrl.Instance.DashPause)
                {
                    GPCtrl.Instance.UICtrl.AttackInputIndicator.ShowIndicatorAt(closestTargetableSpotList[0].transform.position);
                }
                GPCtrl.Instance.CameraThirdPerson.ActivateFreeLook(false);
                //GPCtrl.Instance.CameraLock.CinemachineVirtualCamera.transform.forward = closestWeakSpotList[0].transform.position - transform.position;

                //if (GPCtrl.Instance.CameraLock.CinemachineTargetGroup.m_Targets.Length == 1)
                //{
                //    //GPCtrl.Instance.CameraLock.CinemachineTargetGroup.AddMember(closestWeakSpotList[0].transform, 0.8f, 4.5f);
                //}

            }
            else
            {
                //if (GPCtrl.Instance.CameraLock.CinemachineTargetGroup.m_Targets.Length > 1)
                //{
                GPCtrl.Instance.CameraThirdPerson.ActivateFreeLook(true);
                //GPCtrl.Instance.CameraLock.CinemachineTargetGroup.RemoveMember(GPCtrl.Instance.CameraLock.CinemachineTargetGroup.m_Targets[1].target);
                //}
            }
        } else
        {
            //if (GPCtrl.Instance.CameraLock.CinemachineTargetGroup.m_Targets.Length > 1)
            //{
            //GPCtrl.Instance.CameraLock.CinemachineTargetGroup.RemoveMember(GPCtrl.Instance.CameraLock.CinemachineTargetGroup.m_Targets[1].target);
            GPCtrl.Instance.CameraThirdPerson.ActivateFreeLook(true);
            //}
        }
        if (IsGrappling && _springJoint != null)
        {
            if (Vector3.Distance(transform.position, CurrentTargetSpot.transform.position) < Player.Data.attackStopDistance)
            {
                ReachWeakSpot();
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

    private void ReachWeakSpot()
    {
        Destroy(_springJoint);
        //GPCtrl.Instance.CameraLock.CinemachineTargetGroup.RemoveMember(CurrentWeakSpot.transform);
        GPCtrl.Instance.CameraThirdPerson.ActivateFreeLook(true);
        _springJoint = null;
        IsGrappling = false;
        Player.PlayerSwingingLeft.SwingRopeFX.HideRope(Player.PlayerSwingingLeft.StartSwingLinePoint.position);
        Player.PlayerSwingingRight.SwingRopeFX.HideRope(Player.PlayerSwingingRight.StartSwingLinePoint.position);
        Player.Rigibody.velocity = Vector3.zero;
        Player.Rigibody.useGravity = true;
        Player.SoundData.SFX_Hunter_Attack_Impact.Post(gameObject);
        CurrentTargetSpot.DestroyWeakSpot();
        CurrentTargetSpot = null;
        Player.Collider.enabled = true;
    }
}
