using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwinging : MonoBehaviour
{
    public Player Player;
    private enum Side {  Left, Right };

    [Header("SWINGING")]
    [SerializeField] private ConeRaycast _swingConeRaycast;
    [SerializeField] public Transform StartSwingLinePoint;
    [SerializeField] public Transform EndSwingLinePoint;
    [SerializeField] private Transform BaseSwingAnimation;
    [SerializeField] private PlayerSwinging _otherPlayerSwinging;

    private SpringJoint _springJoint;

    [HideInInspector]
    public bool IsSwinging = false;
    [SerializeField]
    private Side _side;
    [HideInInspector]
    public bool TrySwing = false;
    public LineRenderer SwingLineRenderer;
    [SerializeField] private LayerMask _layerMask;

    private void LateUpdate()
    {
        if (_otherPlayerSwinging.IsSwinging)
        {
            SwingAnimation(_otherPlayerSwinging.EndSwingLinePoint.position);
        }
        if (IsSwinging) //Visual effect for swing line
        {
            SwingAnimation(EndSwingLinePoint.position);
            if (SwingLineRenderer.positionCount == 2)
            {
                SwingLineRenderer.SetPosition(0, StartSwingLinePoint.position);
                if (SwingLineRenderer.GetPosition(1) != EndSwingLinePoint.position)
                {
                    SwingLineRenderer.SetPosition(1, Vector3.Lerp(SwingLineRenderer.GetPosition(1), EndSwingLinePoint.position, 0.1f));
                }
            }
        }
    }

    private void Update()
    {
        if (IsSwinging)
        {
            if (Player.PlayerMovement.CurrentMoveSpeed >= Player.Data.swingSpeed) {
                Camera.main.fieldOfView = (Player.PlayerMovement.CurrentMoveSpeed - Player.Data.swingSpeed) / (Player.Data.swingMaxSpeed - Player.Data.swingSpeed) * Player.Data.fovAddition + 50;
            }
        }

        if (TrySwing && !Player.PlayerAttack.IsGrappling) StartSwing();
        else if (!Player.PlayerDoubleGrappleBoost.IsDoubleGrappling) StopSwing();
    }

    public void CalculateUpVector()
    {
        if (IsSwinging)
        {
            Player.Mesh.up = Vector3.Slerp(Player.Mesh.up, (EndSwingLinePoint.position - Player.transform.position).normalized, Time.deltaTime * 10f);

        }
        else if (Player.PlayerMovement.CurrentMoveSpeed >= Player.Data.swingSpeed)
        {
            Player.Mesh.up = Vector3.Slerp(Player.Mesh.up, Vector3.up, Time.deltaTime * 10f);
        }
    }

    #region Swing

    public void StartSwing()
    {
        if (_springJoint) return;
        _swingConeRaycast.searchPoint = true;
        if (_swingConeRaycast.radius < _swingConeRaycast.maxRadius)
            _swingConeRaycast.radius += Time.deltaTime * Player.Data.radiusDetectionIncreaseSpeed;
        if (_swingConeRaycast.contactPointList.Count > 0)
        {
            float distance = 1000;
            Vector3 point = Vector3.zero;
            for (int i = 0; i < _swingConeRaycast.contactPointList.Count; i++)
            {
                float currentDistance = Vector3.Distance(_swingConeRaycast.perfectPoint.position, _swingConeRaycast.contactPointList[i]);
                if (currentDistance < distance)
                {
                    point = _swingConeRaycast.contactPointList[i];
                    distance = currentDistance;
                }
            }
            if (point == Vector3.zero) return;
            Vector3 direction = point - StartSwingLinePoint.position;
            RaycastHit hit;
            if (Physics.Raycast(StartSwingLinePoint.position, direction, out hit, Player.Data.maxSwingDistance, _layerMask))
            {
                Player.PlayerMovement.CanJumpOnceInAir = true;
                Player.SoundData.SFX_Hunter_Hook_Single_Grappled.Post(EndSwingLinePoint.gameObject);
                Player.SoundData.SFX_Hunter_Hook_Single_Trigger.Post(gameObject);
                IsSwinging = true;
                _swingConeRaycast.searchPoint = false;
                EndSwingLinePoint.SetParent(hit.transform);
                EndSwingLinePoint.position = hit.point;
                Player.Animator.SetBool("isSwinging", true);
                _springJoint = gameObject.AddComponent<SpringJoint>();
                _springJoint.autoConfigureConnectedAnchor = false;
                _springJoint.connectedAnchor = EndSwingLinePoint.position;
                _springJoint.enableCollision = true;
                float distanceFromPoint = Vector3.Distance(StartSwingLinePoint.position, EndSwingLinePoint.position) + 10;
                if (distanceFromPoint < Player.Data.minSwingDistance) distanceFromPoint = Player.Data.minSwingDistance;

                _springJoint.maxDistance = distanceFromPoint * 0.7f;
                _springJoint.minDistance = 0.5f;

                _springJoint.spring = 10;
                _springJoint.damper = 5f;
                _springJoint.massScale = 4.5f;
                SwingLineRenderer.positionCount = 2;
                SwingLineRenderer.SetPosition(1, StartSwingLinePoint.position); //to shoot from the hand of the player
                if (Player.Data.startCurveBoost)
                    Player.Rigibody.AddForce(Vector3.Cross(Player.Mesh.transform.right, (EndSwingLinePoint.position - Player.transform.position).normalized) * Player.Data.startCurveSpeedBoost, ForceMode.Impulse);
            }
        }
    }

    public void StopSwing(bool boost = true, bool destroyVisual = true)
    {
        _swingConeRaycast.radius = _swingConeRaycast.minRadius;
        if (!_springJoint) return;
        Player.SoundData.SFX_Hunter_Hook_Single_Trigger.Post(EndSwingLinePoint.gameObject);
        Player.PlayerMovement.CurrentMoveSpeed++;
        Destroy(_springJoint);
        if (destroyVisual)
        {
            HideLineRenderer();
        }
        IsSwinging = false;
        Player.Animator.SetBool("isSwinging", false);
        float dotProduct = Vector3.Dot(Player.Rigibody.velocity.normalized, Player.Orientation.transform.forward);
        Player.Animator.SetFloat("SwingEndAngle", Player.Rigibody.velocity.normalized.y);
        if (dotProduct > .5f)
        {
            if (Player.Data.endCurveBoost && boost)
            {
                Player.Rigibody.AddForce(Vector3.Cross(Player.Mesh.transform.right, (EndSwingLinePoint.position - Player.transform.position).normalized) * Player.Data.endCurveSpeedBoost, ForceMode.Impulse);
                Player.PlayerMovement.CurrentMoveSpeed++;
            }
        }
    }

    public void HideLineRenderer()
    {
        SwingLineRenderer.positionCount = 0;
        EndSwingLinePoint.parent = null;
    }


    private void SwingAnimation(Vector3 toLook)
    {
        Vector3 dir = (EndSwingLinePoint.position - transform.position).normalized;
        if (_side == Side.Left)
        {
            dir = -dir;
        }
        BaseSwingAnimation.right = dir;
    }

    #endregion
}
