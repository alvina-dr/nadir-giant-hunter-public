using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class Player : MonoBehaviour
{
    public PlayerData Data;
    public PlayerVFXData VFXData;
    public PlayerSoundData SoundData;

    [Header("Player Scripts")]
    public PlayerMovement PlayerMovement;
    public PlayerSwinging PlayerSwingingRight;
    public PlayerSwinging PlayerSwingingLeft;
    public PlayerAttack PlayerAttack;
    public PlayerGrappleBoost PlayerGrappleBoost;
    public PlayerDash PlayerDash;
    public Meshtrail Meshtrail;

    [Header("Components")]
    public Transform Mesh;
    public Rigidbody Rigibody;
    public Transform Orientation;
    public Animator Animator;
    public VisualEffect SparksVFX;
    public CapsuleCollider Collider;
    public VisualEffect DestructionFX;

    [Header("Variables")]
    public float currentTimerPitBottom;

    [Header("Input")]
    public PlayerControls InputManager;
    [SerializeField] private PlayerInput _playerInput;
    private InputAction _attackAction;
    private InputAction _jumpAction;
    private InputAction _swingRightAction;
    private InputAction _swingLeftAction;
    private InputAction _menuAction;

    [Header("Cone Raycast")]
    [SerializeField] private Transform CameraConeRaycastParent;
    [SerializeField] private ConeRaycast CameraRightConeRaycast;
    [SerializeField] private ConeRaycast CameraLeftConeRaycast;
    [SerializeField] private ConeRaycast PlayerRightConeRaycast;
    [SerializeField] private ConeRaycast PlayerLeftConeRaycast;

    private void Awake()
    {
        InputManager = new PlayerControls();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SetupInputActions();
        SetConeRaycast(Data.coneRaycastOnPlayer);
    }

    private void OnEnable()
    {
        InputManager.Enable();
        //InputManager.Gameplay.Lock.started += function => GPCtrl.Instance.CameraThirdPerson.ActivateFreeLook(!GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.enabled);
    }

    private void OnDisable()
    {
        InputManager.Disable();
    }

    private void Update()
    {
        UpdateInputs();
        //CameraConeRaycastParent.transform.forward = Camera.main.transform.forward;
    }

    public void SetConeRaycast(bool onPlayer)
    {
        if (onPlayer)
        {
            PlayerSwingingLeft.SetConeRaycast(PlayerLeftConeRaycast);
            PlayerSwingingRight.SetConeRaycast(PlayerRightConeRaycast);
        } else
        {
            PlayerSwingingLeft.SetConeRaycast(CameraLeftConeRaycast);
            PlayerSwingingRight.SetConeRaycast(CameraRightConeRaycast);
        }
    }

    void SetupInputActions()
    {
        _attackAction = _playerInput.actions["Attack"];
        _jumpAction = _playerInput.actions["Jump"];
        _swingRightAction = _playerInput.actions["SwingRight"];
        _swingLeftAction = _playerInput.actions["SwingLeft"];
        _menuAction = _playerInput.actions["Menu"];
    }

    void UpdateInputs()
    {
        if (_attackAction.WasPressedThisFrame()) //ATTACK
        {
            if (GPCtrl.Instance.Pause) return;
            if (GPCtrl.Instance.DashPause) PlayerDash.Dash();
            else PlayerAttack.Attack();
        }

        if (_jumpAction.WasPressedThisFrame()) //JUMP
        {
            if (GPCtrl.Instance.Pause) return;
            if (PlayerAttack.IsGrappling) return;
            if (PlayerSwingingLeft.IsSwinging || PlayerSwingingRight.IsSwinging)
                PlayerGrappleBoost.Boost();
            else
                PlayerMovement.Jump();
        }

        if (_swingRightAction.WasPressedThisFrame()) //SWING RIGHT STARTED
        {
            if (GPCtrl.Instance.Pause) return;
            if (!PlayerGrappleBoost.IsGrapplingBoost)
                PlayerSwingingRight.IsTrySwing = true;
        } else if (_swingRightAction.WasReleasedThisFrame())
        {
            if (GPCtrl.Instance.Pause) return;
            PlayerSwingingRight.IsTrySwing = false;
        }

        if (_swingLeftAction.WasPressedThisFrame()) //SWING LEFT STARTED
        {
            if (GPCtrl.Instance.Pause) return;
            if (!PlayerGrappleBoost.IsGrapplingBoost)
                PlayerSwingingLeft.IsTrySwing = true;
        } else if(_swingLeftAction.WasReleasedThisFrame())
        {
            if (GPCtrl.Instance.Pause) return;
            PlayerSwingingLeft.IsTrySwing = false;
        }

        if(_menuAction.WasPressedThisFrame())
        {
            GPCtrl.Instance.UICtrl.CallPause();
        }
    }
}