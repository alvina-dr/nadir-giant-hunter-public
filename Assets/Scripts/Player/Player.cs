using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class Player : MonoBehaviour
{
    public PlayerData Data;
    public PlayerDifficultyData DifficultyData;
    public PlayerDifficultyData EasyDifficultyData;
    public PlayerDifficultyData NormalDifficultyData;
    public PlayerDifficultyData HardDifficultyData;
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
    public PlayerInput PlayerInput;
    private InputAction _attackAction;
    private InputAction _jumpAction;
    private InputAction _swingRightAction;
    private InputAction _swingLeftAction;
    private InputAction _menuAction;
    public InputAction MoveAction;

    [Header("Cone Raycast")]
    [SerializeField] private Transform CameraConeRaycastParent;
    [SerializeField] private ConeRaycast CameraRightConeRaycast;
    [SerializeField] private ConeRaycast CameraLeftConeRaycast;
    [SerializeField] private ConeRaycast PlayerRightConeRaycast;
    [SerializeField] private ConeRaycast PlayerLeftConeRaycast;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SetupInputActions();
        SetConeRaycast(Data.coneRaycastOnPlayer);
    }

    private void Update()
    {
        UpdateInputs();
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
        _attackAction = PlayerInput.actions["Attack"];
        _jumpAction = PlayerInput.actions["Jump"];
        _swingRightAction = PlayerInput.actions["SwingRight"];
        _swingLeftAction = PlayerInput.actions["SwingLeft"];
        _menuAction = PlayerInput.actions["Menu"];
        MoveAction = PlayerInput.actions["Move"];
    }

    void UpdateInputs()
    {
        if (TutorialCtrl.Instance.TutoPanelOpen) return;

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