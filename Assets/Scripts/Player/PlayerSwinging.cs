using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using DG.Tweening;

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
    public SwingRopeFX SwingRopeFX;
    [SerializeField] private LayerMask _layerMask;
    private Vector3 _bestSwingPoint;
    private bool _inFirstPartOfSwinging = true;

    private void LateUpdate()
    {
        if (_otherPlayerSwinging.IsSwinging)
        {
            SwingAnimation(_otherHandPosition);
        }
        if (IsSwinging) //Visual effect for swing line
        {
            SwingAnimation(EndSwingLinePoint.position);
            Swinging();
            SwingRopeFX.DrawRope(StartSwingLinePoint.position, EndSwingLinePoint.position);
            _swingConeRaycast.SearchPoint = false;
        }
        else
        {
            _swingConeRaycast.SearchPoint = true;
        }
    }

    private void Update()
    {
        _otherHandPosition = _otherPlayerSwinging.StartSwingLinePoint.position;
        if (!Player.PlayerDash.IsDashing && !Player.PlayerAttack.IsGrappling)
        {
            float factor = (Player.PlayerMovement.CurrentMoveSpeed - Player.Data.swingSpeed) / (Player.Data.swingMaxSpeed - Player.Data.swingSpeed);
            Camera.main.fieldOfView = factor * Player.Data.swingCameraFOVAddition + 50;
            float initialValue = GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(0).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping;
            GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(0).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = Mathf.Lerp(initialValue, factor * Player.Data.swingCameraDistanceAddition, .3f);
            GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(1).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = Mathf.Lerp(initialValue, factor * Player.Data.swingCameraDistanceAddition, .3f);
            GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.GetRig(2).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = Mathf.Lerp(initialValue, factor * Player.Data.swingCameraDistanceAddition, .3f);
            float dotVector = Vector3.Dot((EndSwingLinePoint.position - transform.position).normalized, Vector3.up);
            if (dotVector > .8f)
            {
                //GPCtrl.Instance.CameraThirdPerson.CameraShake.ShakeCamera(2 * dotVector, .1f);
            }
            else if (IsSwinging)
            {
                //GPCtrl.Instance.CameraThirdPerson.CameraShake.StopShake();
            }

            if (dotVector > .3f)
            {
                //DataHolder.Instance.RumbleManager.PulseFor(10f, 10f, .3f);
            } else
            {
                //DataHolder.Instance.RumbleManager.StopPulse();
            }

            if (dotVector > 0.5f) _inFirstPartOfSwinging = false;
        }

        if (IsTrySwing && !Player.PlayerAttack.IsGrappling && !GPCtrl.Instance.DashPause && !Player.PlayerDash.IsDashing) TrySwing();
        else if (!IsTrySwing && !Player.PlayerGrappleBoost.IsGrapplingBoost && !GPCtrl.Instance.DashPause) StopSwing();

        CalculateBestSwingingPoint();

        if (_bestSwingPoint != Vector3.zero && !IsSwinging)
        {
            if (_side == Side.Left)
            {
                GPCtrl.Instance.UICtrl.SwingLeftIndicator.ShowIndicatorAt(_bestSwingPoint);
                GPCtrl.Instance.UICtrl.SwingLeftInput.SetVisible(true);
            }
            else
            {
                GPCtrl.Instance.UICtrl.SwingRightIndicator.ShowIndicatorAt(_bestSwingPoint);
                GPCtrl.Instance.UICtrl.SwingRightInput.SetVisible(true);
            }
        }
        else
        {
            if (_side == Side.Left)
            {
                GPCtrl.Instance.UICtrl.SwingLeftIndicator.HideIndicator();
                GPCtrl.Instance.UICtrl.SwingLeftInput.SetVisible(false);
            }
            else
            {
                GPCtrl.Instance.UICtrl.SwingRightIndicator.HideIndicator();
                GPCtrl.Instance.UICtrl.SwingRightInput.SetVisible(false);
            }
        }
    }

    public void CalculateBestSwingingPoint()
    {
        if (_swingConeRaycast.contactPointList.Count > 0)
        {
            float distance = 1000;

            //Get point closest to perfect point
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
            if (point == Vector3.zero)
            {
                _bestSwingPoint = Vector3.zero;
            } else
            {
                _bestSwingPoint = point;
            }
        }
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
        Debug.DrawRay(Player.transform.position, _swingOriginalDirection*5, UnityEngine.Color.blue);

        Vector3 influenceYPlaned = new Vector3(SwingInfluenceDirection.x, 0, SwingInfluenceDirection.z);
        float dot = Vector3.Dot(_swingOriginalDirection.normalized, influenceYPlaned.normalized);
        Vector3 influence = SwingInfluenceDirection * (dot <= 0 ? 0 : dot + 0.7f);
        Debug.DrawRay(Player.transform.position, influence*5, UnityEngine.Color.green);

        float delta = Mathf.Max(dot, 0);
        delta = delta <= 0 ? 0 : delta + 0.1f;
        delta = Mathf.Min(delta, 1);
        Vector3 mix = Vector3.Lerp(forceAdded * 2000 * Player.Data.SwingBaseOrientationSpeed, influence * 2000 * Player.Data.SwingCameraOrientInfluence, delta);
        Debug.DrawRay(Player.transform.position, mix * 5, UnityEngine.Color.magenta);

        float upDot = Vector3.Dot(Player.Rigibody.velocity.normalized, Player.Orientation.transform.forward);
        float lengthMult = Vector3.Distance(EndSwingLinePoint.position, StartSwingLinePoint.position) * Player.Data.SwingSpeedLengthMult * (upDot - 0.1f);
        Player.Rigibody.AddForce(mix * (1 + lengthMult) * Time.fixedDeltaTime, ForceMode.Force);
        Player.Rigibody.velocity *= 0.999f * (1 + Time.fixedDeltaTime);
        bool hasToStopSwing = Vector3.Dot(Vector3.up, (EndSwingLinePoint.position - Player.transform.position).normalized) <= Player.Data.MaxSwingAngle;
        if (hasToStopSwing && !_inFirstPartOfSwinging)
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

        if (_bestSwingPoint != Vector3.zero) //NORMAL SWINGING
        {
            Vector3 direction = _bestSwingPoint - StartSwingLinePoint.position;
            RaycastHit hit;
            if (Physics.Raycast(StartSwingLinePoint.position, direction, out hit, Player.Data.maxSwingDistance, _layerMask))
            {
                StartSwing(hit.transform, hit.point);
            }
        }
    }

    public void StartSwing(Transform hitTransform, Vector3 hitPoint)
    {
        //swing direction on the y plane
        _swingOriginalDirection = Player.Mesh.forward;
        Player.PlayerMovement.CanJumpOnceInAir = true;
        Player.SoundData.SFX_Hunter_Grapple_Trigger.Post(gameObject);
        IsSwinging = true;
        EndSwingLinePoint.SetParent(hitTransform);
        //if (EndSwingLinePoint.parent != null)
        //    Debug.Log("END SWING LINE POINT PARENT : " + EndSwingLinePoint.parent.name);
        EndSwingLinePoint.position = hitPoint;
        Player.Animator.SetBool("isSwinging", true);
        DataHolder.Instance.RumbleManager.PulseFor(5f, 5f, .1f);
        _inFirstPartOfSwinging = true;

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
        _springJoint.massScale = 100f;

        SwingRopeFX.HideRope(StartSwingLinePoint.position);

        Vector3 newVelocity = Vector3.Cross(Player.Mesh.transform.right, (EndSwingLinePoint.position - Player.transform.position).normalized) * Player.Rigibody.velocity.magnitude;
        //float dot = Vector3.Dot(Player.Mesh.transform.right, (EndSwingLinePoint.position - Player.transform.position).normalized);
        Player.Rigibody.velocity = newVelocity;
        Player.PlayerMovement.CurrentMoveSpeed += Player.PlayerMovement.FallingTimer;
        if (Player.Data.startCurveBoost && EndSwingLinePoint.position.y > Player.transform.position.y - 20)
        {
            Player.Rigibody.AddForce(Vector3.Cross(Player.Mesh.transform.right, (EndSwingLinePoint.position - Player.transform.position).normalized) * (Player.Data.startCurveSpeedBoost + Player.PlayerMovement.FallingTimer * 1000f), ForceMode.Impulse);
            Debug.Log("START BOOST");
        }
    }

    public void StopSwing(bool boost = true, bool destroyVisual = true)
    {
        if (!_springJoint) return;
        Player.SoundData.SFX_Hunter_Grapple_Release.Post(EndSwingLinePoint.gameObject);
        Player.PlayerMovement.CurrentMoveSpeed++;
        Destroy(_springJoint);
        if (destroyVisual)
        {
            SwingRopeFX.HideRope(StartSwingLinePoint.position);
        }
        IsSwinging = false;
        Player.Animator.SetBool("isSwinging", false);
        float dotProduct = Vector3.Dot(Player.Rigibody.velocity.normalized, Player.Orientation.transform.forward);
        Player.Animator.SetFloat("SwingEndAngle", Player.Rigibody.velocity.normalized.y);
        EndSwingLinePoint.transform.parent = null;

        if (Player.Data.endCurveBoost && boost)
        {
            Player.PlayerMovement.CurrentMoveSpeed++;
        }
        if (Player.Rigibody.velocity.y > Player.Data.maxYForceOnRelease)
        {
            Player.Rigibody.velocity = new Vector3(Player.Rigibody.velocity.x, Player.Data.maxYForceOnRelease, Player.Rigibody.velocity.z);
        }
        Vector3 flatSpeed = new Vector3(Player.Rigibody.velocity.x, 0, Player.Rigibody.velocity.z).normalized;
        //Player.Rigibody.AddForce(flatSpeed * Player.Data.endCurveSpeedBoost * Player.PlayerMovement.CurrentMoveSpeed / 10, ForceMode.Impulse); // 
        Player.Rigibody.AddForce(Camera.main.transform.forward * Player.Data.endCurveSpeedBoost, ForceMode.Impulse);
        DataHolder.Instance.RumbleManager.PulseFor(5f, 5f, .1f);
    }

    private void SwingAnimation(Vector3 toLook)
    {
        Vector3 dir = (toLook - BaseSwingAnimation.position).normalized;
        Debug.DrawRay(BaseSwingAnimation.position, dir);
        BaseSwingAnimation.up = dir;
    }

    public void SetConeRaycast(ConeRaycast coneRaycast)
    {
        _swingConeRaycast = coneRaycast;
    }
    #endregion
}