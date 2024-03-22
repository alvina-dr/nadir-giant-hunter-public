using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwinging : MonoBehaviour
{
    public Player Player;

    [Header("SWINGING")]
    [SerializeField] private ConeRaycast _swingConeRaycast;
    [SerializeField] private Transform _startSwingLinePoint;
    [SerializeField] private Transform _endSwingLinePoint;

    private SpringJoint _springJoint;

    [HideInInspector]
    public bool IsSwinging = false;
    [HideInInspector]
    public bool TrySwing = false;
    public LineRenderer SwingLineRenderer;
    [SerializeField] private LayerMask _layerMask;

    private void LateUpdate()
    {
        if (_springJoint) //Visual effect for swing line
        {
            SwingLineRenderer.SetPosition(0, _startSwingLinePoint.position);
            if (SwingLineRenderer.GetPosition(1) != _endSwingLinePoint.position)
            {
                SwingLineRenderer.SetPosition(1, Vector3.Lerp(SwingLineRenderer.GetPosition(1), _endSwingLinePoint.position, 0.1f));
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
        } else if (Player.PlayerMovement.CurrentMoveSpeed >= Player.Data.swingSpeed)
        {
            //Player.PlayerMovement.CurrentMoveSpeed *= Player.Data.airSlowDown;
        }

        if (TrySwing) StartSwing();
        else StopSwing();
        Debug.DrawRay(Player.transform.position, Vector3.Cross(Player.Mesh.transform.right, (_endSwingLinePoint.position - Player.transform.position).normalized) * 4);
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
            Vector3 direction = point - _startSwingLinePoint.position;
            RaycastHit hit;
            if (Physics.Raycast(_startSwingLinePoint.position, direction, out hit, Player.Data.maxSwingDistance, _layerMask))
            {
                IsSwinging = true;
                _swingConeRaycast.searchPoint = false;
                _endSwingLinePoint.SetParent(hit.transform);
                _endSwingLinePoint.position = hit.point;
                _springJoint = gameObject.AddComponent<SpringJoint>();
                _springJoint.autoConfigureConnectedAnchor = false;
                _springJoint.connectedAnchor = _endSwingLinePoint.position;
                _springJoint.enableCollision = true;
                float distanceFromPoint = Vector3.Distance(_startSwingLinePoint.position, _endSwingLinePoint.position) + 10;
                if (distanceFromPoint < Player.Data.minLineDistance) distanceFromPoint = Player.Data.minLineDistance;

                _springJoint.maxDistance = distanceFromPoint * 0.8f;
                _springJoint.minDistance = 0.5f;

                _springJoint.spring = 10;
                _springJoint.damper = 5f;
                _springJoint.massScale = 4.5f;
                SwingLineRenderer.positionCount = 2;
                SwingLineRenderer.SetPosition(1, _startSwingLinePoint.position); //to shoot from the hand of the player
                if (Player.Data.startCurveBoost)
                    Player.Rigibody.AddForce(Vector3.Cross(Player.Mesh.transform.right, (_endSwingLinePoint.position - Player.transform.position).normalized) * Player.Data.startCurveSpeedBoost, ForceMode.Impulse);
            }
        }
    }

    public void StopSwing()
    {
        _swingConeRaycast.radius = _swingConeRaycast.minRadius;
        if (!_springJoint) return;
        Player.PlayerMovement.CurrentMoveSpeed++;
        Destroy(_springJoint);
        SwingLineRenderer.positionCount = 0;
        IsSwinging = false;
        _endSwingLinePoint.parent = null;
        if (Vector3.Dot(Player.Rigibody.velocity, Player.Orientation.transform.forward) > .5f)
        {
            if (Player.Data.endCurveBoost)
            {
                Player.Rigibody.AddForce(Vector3.Cross(Player.Mesh.transform.right, (_endSwingLinePoint.position - Player.transform.position).normalized) * Player.Data.endCurveSpeedBoost, ForceMode.Impulse);
                Player.PlayerMovement.CurrentMoveSpeed++;
            }
        }
    }
    #endregion
}
