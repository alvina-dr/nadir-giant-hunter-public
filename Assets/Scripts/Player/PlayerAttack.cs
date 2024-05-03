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
    public List<TargetableSpot> closestWeakSpotList = new List<TargetableSpot>();

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
        Player.PlayerSwingingLeft.StopSwing(false);
        Player.PlayerSwingingRight.StopSwing(false);
        IsGrappling = true;
        CurrentTargetSpot = weakSpot;
        Player.Rigibody.useGravity = false;
        Player.Rigibody.velocity = Vector3.zero;
        _springJoint = gameObject.AddComponent<SpringJoint>();
        _springJoint.autoConfigureConnectedAnchor = false;
        _springJoint.connectedAnchor = Vector3.zero;
        _springJoint.connectedBody = weakSpot.Rigidbody;

        _springJoint.spring = 30;
        _springJoint.damper = 0f;
        _springJoint.massScale = Player.Data.dragForce;

        Player.Animator.SetTrigger("Attack");
        Player.PlayerSwingingLeft.SwingLineRenderer.positionCount = 2;
        Player.PlayerSwingingLeft.SwingLineRenderer.SetPosition(1, weakSpot.transform.position); //to shoot from the hand of the player
        Player.PlayerSwingingRight.SwingLineRenderer.positionCount = 2;
        Player.PlayerSwingingRight.SwingLineRenderer.SetPosition(1, weakSpot.transform.position); //to shoot from the hand of the player
        Player.SoundData.SFX_Hunter_Attack_Rush.Post(gameObject);
    }

    private void Update()
    {
        closestWeakSpotList = GPCtrl.Instance.TargetableSpotList;
        closestWeakSpotList.Sort(delegate (TargetableSpot a, TargetableSpot b)
        {
            return Vector3.Distance(this.transform.position, a.transform.position).CompareTo(Vector3.Distance(this.transform.position, b.transform.position));
        });
        GPCtrl.Instance.UICtrl.AttackInputIndication.HideIndicator();
        if (closestWeakSpotList.Count > 0 ) {
            if (Vector3.Distance(transform.position, closestWeakSpotList[0].transform.position) < Player.Data.weakSpotDetectionDistance)
            {
                GPCtrl.Instance.UICtrl.AttackInputIndication.ShowIndicatorAt(closestWeakSpotList[0].transform.position);
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
                Destroy(_springJoint);
                //GPCtrl.Instance.CameraLock.CinemachineTargetGroup.RemoveMember(CurrentWeakSpot.transform);
                GPCtrl.Instance.CameraThirdPerson.ActivateFreeLook(true);
                _springJoint = null;
                IsGrappling = false;
                Player.PlayerSwingingLeft.SwingLineRenderer.positionCount = 0;
                Player.PlayerSwingingRight.SwingLineRenderer.positionCount = 0;
                Player.Rigibody.velocity = Vector3.zero;
                Player.Rigibody.useGravity = true;
                Player.SoundData.SFX_Hunter_Attack_Impact.Post(gameObject);
                CurrentTargetSpot.DestroyWeakSpot();
                CurrentTargetSpot = null;
            }
        }
    }

    private void LateUpdate()
    {
        if (_springJoint && IsGrappling) //Visual effect for swing line
        {
            if (Player.PlayerSwingingLeft.SwingLineRenderer.positionCount == 2)
            {
                Player.PlayerSwingingLeft.SwingLineRenderer.SetPosition(0, Player.PlayerSwingingLeft.StartSwingLinePoint.position);
                if (Player.PlayerSwingingLeft.SwingLineRenderer.GetPosition(1) != CurrentTargetSpot.transform.position)
                {
                    Player.PlayerSwingingLeft.SwingLineRenderer.SetPosition(1, Vector3.Lerp(Player.PlayerSwingingLeft.SwingLineRenderer.GetPosition(1), CurrentTargetSpot.transform.position, 0.1f));
                }
            }
            if (Player.PlayerSwingingRight.SwingLineRenderer.positionCount == 2)
            {
                Player.PlayerSwingingRight.SwingLineRenderer.SetPosition(0, Player.PlayerSwingingRight.StartSwingLinePoint.position);
                if (Player.PlayerSwingingRight.SwingLineRenderer.GetPosition(1) != CurrentTargetSpot.transform.position)
                {
                    Player.PlayerSwingingRight.SwingLineRenderer.SetPosition(1, Vector3.Lerp(Player.PlayerSwingingRight.SwingLineRenderer.GetPosition(1), CurrentTargetSpot.transform.position, 0.1f));
                }
            }
        }
    }
}
