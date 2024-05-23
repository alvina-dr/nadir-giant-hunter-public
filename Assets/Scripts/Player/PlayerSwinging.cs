using Cinemachine;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
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
    private Vector3 _otherHandPosition;
    [SerializeField] private Vector3 _RotationToAvoid;
    [SerializeField] private float _MaxAngleFromAvoid;
    //the direction of the swing when started to swing.
    [SerializeField, ReadOnly] private Vector3 _swingOriginalDirection;
    [ReadOnly] public Vector3 SwingInfluenceDirection;

    private SpringJoint _springJoint;

    [HideInInspector]
    public bool IsSwinging = false;
    [SerializeField]
    private Side _side;
    [HideInInspector]
    public bool IsTrySwing = false;
    public LineRenderer SwingLineRenderer;
    [SerializeField] private LayerMask _layerMask;

    private void LateUpdate()
    {
        if (_otherPlayerSwinging.IsSwinging)
        {
            SwingAnimation(_otherHandPosition);
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
        if (IsSwinging)
        {
            Swinging();
        }
    }

    private void Update()
    {
        _otherHandPosition = _otherPlayerSwinging.StartSwingLinePoint.position;
        if (!Player.PlayerDash.IsDashing)
        {
            float factor = (Player.PlayerMovement.CurrentMoveSpeed - Player.Data.swingSpeed) / (Player.Data.swingMaxSpeed - Player.Data.swingSpeed);
            Camera.main.fieldOfView = factor * Player.Data.swingCameraFOVAddition + 50;
            float initialValue = GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(0).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping;
            GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(0).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = Mathf.Lerp(initialValue, factor * Player.Data.swingCameraDistanceAddition, .3f);
            GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(1).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = Mathf.Lerp(initialValue, factor * Player.Data.swingCameraDistanceAddition, .3f);
            GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(2).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = Mathf.Lerp(initialValue, factor * Player.Data.swingCameraDistanceAddition, .3f);
            float dotVector = Vector3.Dot((EndSwingLinePoint.position - transform.position).normalized, Vector3.up);
            //Debug.Log("dot vector : " + dotVector);
            if (dotVector > .8f)
            {
                //Debug.Log("SHAKKEEEE");
                //GPCtrl.Instance.CameraThirdPerson.CameraShake.ShakeCamera(2 * dotVector, .1f);
            } else if (IsSwinging)
            {
                //GPCtrl.Instance.CameraThirdPerson.CameraShake.StopShake();
            }
        }
        if (_side == Side.Right)
        {
            //Debug.Log("try swing : " + IsTrySwing);
        }
        if (IsTrySwing && !Player.PlayerAttack.IsGrappling && !GPCtrl.Instance.DashPause) TrySwing();
        else if (!IsTrySwing && !Player.PlayerGrappleBoost.IsGrapplingBoost && !GPCtrl.Instance.DashPause) StopSwing();
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

    public void Swinging()
    {
        Vector3 forceAdded = new Vector3(_swingOriginalDirection.x, 0, _swingOriginalDirection.z);
        Debug.DrawRay(Player.transform.position, _swingOriginalDirection*5, Color.blue);

        Vector3 influenceYPlaned = new Vector3(SwingInfluenceDirection.x, 0, SwingInfluenceDirection.z);
        float dot = Vector3.Dot(_swingOriginalDirection.normalized, influenceYPlaned.normalized);
        Vector3 influence = SwingInfluenceDirection * (dot <= 0 ? 0 : dot+0.7f);
        Debug.DrawRay(Player.transform.position, influence*5, Color.green);

        float delta = Mathf.Max(dot, 0);
        delta = delta <= 0 ? 0 : delta+0.1f;
        delta = Mathf.Min(delta, 1);
        Vector3 mix = Vector3.Lerp(forceAdded * 2000 * Player.Data.SwingBaseOrientationSpeed, influence * 2000 * Player.Data.SwingCameraOrientInfluence, delta);
        Debug.DrawRay(Player.transform.position, mix * 5, Color.magenta);

        float upDot = Vector3.Dot(Player.Rigibody.velocity.normalized, Player.Orientation.transform.forward);
        float lengthMult = Vector3.Distance(EndSwingLinePoint.position, StartSwingLinePoint.position) * Player.Data.SwingSpeedLengthMult * (upDot-0.1f);
        Player.Rigibody.AddForce(mix * (1+lengthMult) * Time.fixedDeltaTime, ForceMode.Force);
        Player.Rigibody.velocity *= 0.999f * (1+Time.deltaTime);
        bool hasToStopSwing = Vector3.Dot(Vector3.up, (EndSwingLinePoint.position - Player.transform.position).normalized) <= Player.Data.MaxSwingAngle;
        if (hasToStopSwing)
        {
            IsTrySwing = false;
            StopSwing();
        }
    }

    public void TrySwing()
    {
        if (_springJoint) return;
        if (Player.Data.magicSwinging) //MAGIC SWINGING
        {
            StartSwing(null, _swingConeRaycast.perfectPoint.position);
            return;
        }
        _swingConeRaycast.searchPoint = true;
        if (_swingConeRaycast.radius < _swingConeRaycast.maxRadius)
            _swingConeRaycast.radius += Time.deltaTime * Player.Data.radiusDetectionIncreaseSpeed;
        if (_swingConeRaycast.contactPointList.Count > 0)
        {
            float distance = 1000;
            Vector3 point = Vector3.zero;
            //Get point closest to perfect point
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
                StartSwing(hit.transform, hit.point);
            }
        }
    }

    public void StartSwing(Transform hitTransform, Vector3 hitPoint)
    {
        //Debug.Log("start swinging");
        //swing direction on the y plane
        _swingOriginalDirection = Player.Mesh.forward;//new Vector3(Player.Rigibody.velocity.normalized.x, 0, Player.Rigibody.velocity.normalized.z);
        Player.PlayerMovement.CanJumpOnceInAir = true;
        Player.SoundData.SFX_Hunter_Hook_Single_Grappled.Post(EndSwingLinePoint.gameObject);
        Player.SoundData.SFX_Hunter_Hook_Single_Trigger.Post(gameObject);
        IsSwinging = true;
        _swingConeRaycast.searchPoint = false;
        EndSwingLinePoint.SetParent(hitTransform);
        EndSwingLinePoint.position = hitPoint;
        Player.Animator.SetBool("isSwinging", true);
        _springJoint = gameObject.AddComponent<SpringJoint>();
        _springJoint.autoConfigureConnectedAnchor = false;
        _springJoint.connectedAnchor = EndSwingLinePoint.position;
        _springJoint.enableCollision = true;
        float distanceFromPoint = Vector3.Distance(StartSwingLinePoint.position, EndSwingLinePoint.position) + 10;
        if (distanceFromPoint < Player.Data.minSwingDistance) distanceFromPoint = Player.Data.minSwingDistance;

        _springJoint.maxDistance = distanceFromPoint * 0.7f;
        _springJoint.minDistance = 0.5f;

        _springJoint.spring = 0;
        _springJoint.damper = 5f;
        _springJoint.massScale = 4.5f;
        SwingLineRenderer.positionCount = 2;
        SwingLineRenderer.SetPosition(1, StartSwingLinePoint.position); //to shoot from the hand of the
        //Player.Rigibody.velocity = Vector3.zero;
        if (Player.Data.startCurveBoost)
            Player.Rigibody.AddForce(Vector3.Cross(Player.Mesh.transform.right, (EndSwingLinePoint.position - Player.transform.position).normalized) * Player.Data.startCurveSpeedBoost, ForceMode.Impulse);
    }

    public void StopSwing(bool boost = true, bool destroyVisual = true)
    {
        if (_side == Side.Right)
        {
            //Debug.Log("stop swinging");
        }
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
        if (Player.Data.endCurveBoost && boost)
        {
            Player.Rigibody.AddForce(Vector3.Cross(Player.Mesh.transform.right, (EndSwingLinePoint.position - Player.transform.position).normalized) * Player.Data.endCurveSpeedBoost, ForceMode.Impulse);
            Player.PlayerMovement.CurrentMoveSpeed++;
        }
    }

    public void HideLineRenderer()
    {
        SwingLineRenderer.positionCount = 0;
        EndSwingLinePoint.parent = null;
    }


    private void SwingAnimation(Vector3 toLook)
    {
        Vector3 dir = (toLook - BaseSwingAnimation.position).normalized;
        Debug.DrawRay(BaseSwingAnimation.position, dir);
        BaseSwingAnimation.up = dir;
    }

    #endregion
}
