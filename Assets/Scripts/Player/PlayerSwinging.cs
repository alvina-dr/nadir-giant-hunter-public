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
    [SerializeField] private LayerMask layerMask;

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
        if (isSwinging)
        {
            //if (player.rigibody.velocity.y < 1 && player.rigibody.velocity.y > -1) Debug.Log("VELOCITY ALMOST NULL");
            //player.mesh.transform.up = endSwingLinePoint.position - player.mesh.transform.position;
            if (player.playerMovement.currentMoveSpeed >= player.data.swingSpeed) {
                Camera.main.fieldOfView = (player.playerMovement.currentMoveSpeed - player.data.swingSpeed) / (player.data.swingMaxSpeed - player.data.swingSpeed) * player.data.fovAddition + 50;
            }
        } else if (player.playerMovement.currentMoveSpeed >= player.data.swingSpeed)
        {
            player.playerMovement.currentMoveSpeed *= player.data.airSlowDown;
        }

        if (trySwing) StartSwing();
        else StopSwing();
        Debug.DrawRay(player.transform.position, Vector3.Cross(player.mesh.transform.right, (endSwingLinePoint.position - player.transform.position).normalized) * 4);
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
        //if (player.rigibody.velocity.y > 0) return;
        swingConeRaycast.searchPoint = true;
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
            if (_point == Vector3.zero) return;
            Vector3 _direction = _point - startSwingLinePoint.position;
            RaycastHit hit;
            if (Physics.Raycast(startSwingLinePoint.position, _direction, out hit, player.data.maxSwingDistance, layerMask))
            {
                isSwinging = true;
                swingConeRaycast.searchPoint = false;
                endSwingLinePoint.SetParent(hit.transform);
                endSwingLinePoint.position = hit.point;
                springJoint = gameObject.AddComponent<SpringJoint>();
                springJoint.autoConfigureConnectedAnchor = false;
                springJoint.connectedAnchor = endSwingLinePoint.position;
                springJoint.enableCollision = true;
                float _distanceFromPoint = Vector3.Distance(startSwingLinePoint.position, endSwingLinePoint.position) + 10;
                if (_distanceFromPoint < player.data.minLineDistance) _distanceFromPoint = player.data.minLineDistance;

                springJoint.maxDistance = _distanceFromPoint * 0.8f;
                springJoint.minDistance = _distanceFromPoint * 0.25f;

                springJoint.spring = 10;
                springJoint.damper = 5f;
                springJoint.massScale = 4.5f;
                swingLineRenderer.positionCount = 2;
                swingLineRenderer.SetPosition(1, startSwingLinePoint.position); //to shoot from the hand of the player
                if (player.data.startCurveBoost)
                    player.rigibody.AddForce(Vector3.Cross(player.mesh.transform.right, (endSwingLinePoint.position - player.transform.position).normalized) * player.data.startCurveSpeedBoost, ForceMode.Impulse);
            }
        }
    }

    public void StopSwing()
    {
        swingConeRaycast.radius = swingConeRaycast.minRadius;
        if (!springJoint) return;
        player.playerMovement.currentMoveSpeed++;
        Destroy(springJoint);
        swingLineRenderer.positionCount = 0;
        isSwinging = false;
        endSwingLinePoint.parent = null;
        if (Vector3.Dot(player.rigibody.velocity, player.orientation.transform.forward) > .5f)
        {
            if (player.data.endCurveBoost)
            {
                player.rigibody.AddForce(Vector3.Cross(player.mesh.transform.right, (endSwingLinePoint.position - player.transform.position).normalized) * player.data.endCurveSpeedBoost, ForceMode.Impulse);
                player.playerMovement.currentMoveSpeed++;
            }
        }
    }
    #endregion
}
