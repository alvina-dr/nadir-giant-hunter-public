using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwinging : MonoBehaviour
{
    public Player player;

    [Header("SWINGING")]
    [SerializeField] private ConeRaycast swingConeRaycast;
    [SerializeField] private Transform startSwingLinePoint;
    [SerializeField] private Transform endSwingLinePoint;

    private SpringJoint springJoint;
    [HideInInspector]
    public bool isSwinging = false;
    [HideInInspector]
    public bool trySwing = false;
    [SerializeField] private LineRenderer swingLineRenderer;

    //Visual effect for swing line
    private void LateUpdate()
    {
        if (springJoint)
        {
            swingLineRenderer.SetPosition(0, startSwingLinePoint.position);
            if (swingLineRenderer.GetPosition(1) != endSwingLinePoint.position)
            {
                swingLineRenderer.SetPosition(1, Vector3.Lerp(swingLineRenderer.GetPosition(1), endSwingLinePoint.position, 0.1f));
            }
        }
        //if (isGrappling)
        //{
        //    if (leftGrapplingLineRenderer.positionCount > 0)
        //    {
        //        leftGrapplingLineRenderer.SetPosition(0, player.leftHand.transform.position);
        //        if (leftGrapplingLineRenderer.GetPosition(1) != grapplePoint.transform.position)
        //        {
        //            leftGrapplingLineRenderer.SetPosition(1, Vector3.Lerp(leftGrapplingLineRenderer.GetPosition(1), grapplePoint.transform.position, 0.1f));
        //        }
        //    }
        //    if (rightGrapplingLineRenderer.positionCount > 0)
        //    {
        //        rightGrapplingLineRenderer.SetPosition(0, player.rightHand.transform.position);
        //        if (rightGrapplingLineRenderer.GetPosition(1) != grapplePoint.transform.position)
        //        {
        //            rightGrapplingLineRenderer.SetPosition(1, Vector3.Lerp(rightGrapplingLineRenderer.GetPosition(1), grapplePoint.transform.position, 0.1f));
        //        }
        //    }
        //}
    }

    private void Update()
    {
        //if (isSwinging)
        //{
        //    player.mesh.transform.up = endSwingLinePoint.position - player.mesh.transform.position;
        //}

        if (trySwing) StartSwing();
        else StopSwing();

        if (player.playerSwinging.isSwinging)
        {
            if (player.playerMovement.currentMoveSpeed < player.data.swingSpeed) player.playerMovement.currentMoveSpeed = player.data.swingSpeed;
            player.playerMovement.currentMoveSpeed += player.data.swingAcceleration * Time.deltaTime;
            if (player.playerMovement.currentMoveSpeed >= player.data.swingMaxSpeed) player.playerMovement.currentMoveSpeed = player.data.swingMaxSpeed;
        }
        //if (grappleFreeze)
        //{
        //    player.rigibody.velocity = Vector3.zero;
        //}

        //if (isGrappling && Vector3.Distance(transform.position, grapplePoint.transform.position) < 2)
        //{
        //    StopGrapple();
        //}
        //if (!isGrappling) GPCtrl.Instance.grapplePointIndicator.Hide();
        //if (grapplingConeCollider.pointList.Count > 0 && !isGrappling)
        //{
        //    GrapplingPoint _point = grapplingConeCollider.GetBestPoint();
        //    Vector3 _direction = _point.transform.position - player.playerMesh.transform.position;
        //    RaycastHit hit;
        //    if (Physics.Raycast(player.playerMesh.transform.position, _direction, out hit, maxSwingingDistance))
        //    {
        //        if (hit.transform.GetComponent<GrapplingPoint>() != null)
        //        {
        //            grapplePoint = _point;
        //            GPCtrl.Instance.grapplePointIndicator.ShowAtWorldPosition(hit.point);
        //        }
        //    }
        //}

        //if (player.rightShoulder && player.leftShoulder && grapplePoint != null && !isGrappling)
        //{
        //    StartGrapple();
        //}
    }

    #region Swing

    public void StartSwing()
    {
        if (springJoint) return;
        if (player.rigibody.velocity.y > 0) return;
        if (swingConeRaycast.radius < swingConeRaycast.maxRadius)
            swingConeRaycast.radius += Time.deltaTime * player.data.radiusDetectionIncreaseSpeed;
        if (swingConeRaycast.contactPointList.Count > 0)
        {
            float _distance = 1000;
            Vector3 _point = Vector3.zero;
            for (int i = 0; i < swingConeRaycast.contactPointList.Count; i++)
            {
                float _currentDistance = Vector3.Distance(swingConeRaycast.perfectPoint.position, swingConeRaycast.contactPointList[i]);
                if (_currentDistance < _distance)
                {
                    _point = swingConeRaycast.contactPointList[i];
                    _distance = _currentDistance;
                }
            }
            Vector3 _direction = _point - GPCtrl.Instance.player.mesh.transform.position;
            RaycastHit hit;
            if (Physics.Raycast(GPCtrl.Instance.player.mesh.transform.position, _direction, out hit, player.data.maxSwingDistance))
            {
                isSwinging = true;
                endSwingLinePoint.SetParent(hit.transform);
                endSwingLinePoint.position = hit.point;
                springJoint = gameObject.AddComponent<SpringJoint>();
                springJoint.autoConfigureConnectedAnchor = false;
                springJoint.connectedAnchor = endSwingLinePoint.position;

                float _distanceFromPoint = Vector3.Distance(transform.position, endSwingLinePoint.position) + 10;

                springJoint.maxDistance = _distanceFromPoint * 0.8f;
                springJoint.minDistance = _distanceFromPoint * 0.25f;

                springJoint.spring = 10;
                springJoint.damper = 5f;
                springJoint.massScale = 4.5f;
                swingLineRenderer.positionCount = 2;
            }
        }
    }

    public void StopSwing()
    {
        if (!springJoint) return;
        swingConeRaycast.radius = swingConeRaycast.minRadius;
        GPCtrl.Instance.player.playerMovement.currentMoveSpeed++;
        Destroy(springJoint);
        swingLineRenderer.positionCount = 0;
        isSwinging = false;
        endSwingLinePoint.parent = null;
        Debug.Log("SCALAR : " + Vector3.Dot(player.rigibody.velocity, player.orientation.transform.forward));
        if (Vector3.Dot(player.rigibody.velocity, player.orientation.transform.forward) > .5f)
        {
            Debug.Log("add acceleration");
            player.rigibody.AddForce(player.rigibody.velocity.normalized * player.data.endCurveSpeedBoost, ForceMode.Impulse);
        }
    }
    #endregion
}
