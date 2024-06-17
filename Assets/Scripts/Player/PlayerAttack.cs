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
    public TargetableSpot BestTargetSpotFit;
    public List<TargetableSpot> ClosestTargetableSpotList = new List<TargetableSpot>();
    public Vector3 TargetSpotDistance = Vector3.zero;
    public LayerMask attackLayer;

    public void Attack()
    {
        if (IsGrappling) return;
        if (BestTargetSpotFit == null) return;
        GrappleWeakSpot(BestTargetSpotFit);
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
        //UI setup
        GPCtrl.Instance.UICtrl.AttackInputIndicator.SetupAppearance(true, false);
        GPCtrl.Instance.UICtrl.MonsterHighIndicator.SetupAppearance(false, true);
        GPCtrl.Instance.UICtrl.AttackInput.SetVisible(false);
        GPCtrl.Instance.UICtrl.WeakSpotContextUI.gameObject.SetActive(false);
        GPCtrl.Instance.UICtrl.BumperContextUI.gameObject.SetActive(false);
        GPCtrl.Instance.UICtrl.DashContextUI.gameObject.SetActive(false);
        BestTargetSpotFit = null;

        ClosestTargetableSpotList = GPCtrl.Instance.TargetableSpotList;

        //only keep close enough targetable spots
        List<TargetableSpot> closeEnoughSpotList = new List<TargetableSpot>();
        for (int i = 0; i < ClosestTargetableSpotList.Count; i++) 
        {
            if (Vector3.Distance(ClosestTargetableSpotList[i].transform.position, transform.position) < Player.DifficultyData.TargetableSpotDetectionDistance)
            {
                closeEnoughSpotList.Add(ClosestTargetableSpotList[i]);
            } 
        }

        //sort close enough targetable spots from closest to farthest
        closeEnoughSpotList.Sort(delegate (TargetableSpot a, TargetableSpot b) 
        {
            return Vector3.Distance(this.transform.position, a.transform.position).CompareTo(Vector3.Distance(this.transform.position, b.transform.position));
        });

        if (GPCtrl.Instance.UICtrl != null)
            GPCtrl.Instance.UICtrl.AttackInputIndicator.HideIndicator();

        if (closeEnoughSpotList.Count > 0 ) {
            if (!GPCtrl.Instance.DashPause)
            {

                //Debug.Log("close enough spot list : " + closeEnoughSpotList.Count);

                //Check if there are no collision between point and player
                List<TargetableSpot> noCollisionBetweenSpotList = new List<TargetableSpot>();
                for (int i = 0; i < closeEnoughSpotList.Count; i++)
                {
                    Vector3 direction = closeEnoughSpotList[i].transform.position - transform.position;
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, direction, out hit, Player.DifficultyData.TargetableSpotDetectionDistance, attackLayer))
                    {
                        if (hit.transform.gameObject == closeEnoughSpotList[i].gameObject)
                        {
                            noCollisionBetweenSpotList.Add(closeEnoughSpotList[i]);
                        }
                    }
                }

                //Debug.Log("no collision spot list : " + noCollisionBetweenSpotList.Count);
                if (noCollisionBetweenSpotList.Count > 0 ) {

                    //verify if there's a point in the center of the screen that would be a better match
                    TargetableSpot bestSpotFitForAttack = noCollisionBetweenSpotList[0];
                    float matchingDotProduct = 0;
                    for (int i = 0; i < noCollisionBetweenSpotList.Count; i++)
                    {
                        Vector3 cameraToPoint = (noCollisionBetweenSpotList[i].transform.position - Camera.main.transform.position).normalized;
                        float dotProduct = Vector3.Dot(cameraToPoint, Camera.main.transform.forward);
                        //if (noCollisionBetweenSpotList.Count > 1) Debug.Log("dot product : " + dotProduct + " for " + noCollisionBetweenSpotList[i].name);
                        if (dotProduct > Player.Data.centerOfScreenSpotPriority)
                        {
                            if (dotProduct > matchingDotProduct) //there's a point thats more in the center of the screen
                            {
                                matchingDotProduct = dotProduct;
                                bestSpotFitForAttack = noCollisionBetweenSpotList[i];
                            }
                        }
                    }

                    BestTargetSpotFit = bestSpotFitForAttack;
                    GPCtrl.Instance.UICtrl.AttackInput.SetVisible(true);
                    if (GPCtrl.Instance.EnemySpawner != null && GPCtrl.Instance.EnemySpawner.EnemyList.Count > 0 && bestSpotFitForAttack == GPCtrl.Instance.EnemySpawner.EnemyList[0].EnemyWeakSpotManagement.WeakSpotList[0])
                    {
                        GPCtrl.Instance.UICtrl.AttackInputIndicator.SetupAppearance(true, true);
                        GPCtrl.Instance.UICtrl.MonsterHighIndicator.SetupAppearance(false, false);
                    }
                    GPCtrl.Instance.UICtrl.AttackInputIndicator.ShowIndicatorAt(bestSpotFitForAttack.transform.position);
                    GPCtrl.Instance.UICtrl.AttackInputIndicator.SetTargetableSpotType(bestSpotFitForAttack.SpotCurrentType);
                    GPCtrl.Instance.UICtrl.MonsterHighIndicator.SetTargetableSpotType(bestSpotFitForAttack.SpotCurrentType);
                    switch (bestSpotFitForAttack.SpotCurrentType)
                    {
                        case TargetableSpot.SpotType.WeakSpot:
                            GPCtrl.Instance.UICtrl.WeakSpotContextUI.gameObject.SetActive(true);
                            break;
                        case TargetableSpot.SpotType.DashSpot:
                            GPCtrl.Instance.UICtrl.DashContextUI.gameObject.SetActive(true);
                            break;
                        case TargetableSpot.SpotType.Bumper:
                            GPCtrl.Instance.UICtrl.BumperContextUI.gameObject.SetActive(true);
                            break;

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
        IsGrappling = false;
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