using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Player : MonoBehaviour
{
    public PlayerData Data;
    public PlayerVFXData VFXData;
    public PlayerSoundData SoundData;
    public PlayerControls InputManager;

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

    [Header("Variables")]
    public float currentTimerPitBottom;

    private void Awake()
    {
        InputManager = new PlayerControls();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        InputManager.Enable();
        InputManager.Gameplay.SwingRight.started += function => {
            if (GPCtrl.Instance.Pause) return;
            if (!PlayerGrappleBoost.IsGrapplingBoost)
                PlayerSwingingRight.IsTrySwing = true; 
        };
        InputManager.Gameplay.SwingRight.canceled += function => {
            if (GPCtrl.Instance.Pause) return;
            PlayerSwingingRight.IsTrySwing = false;
        };
        InputManager.Gameplay.SwingLeft.started += function => {
            if (GPCtrl.Instance.Pause) return;
            if (!PlayerGrappleBoost.IsGrapplingBoost)
                PlayerSwingingLeft.IsTrySwing = true; 
        };
        InputManager.Gameplay.SwingLeft.canceled += function => {
            if (GPCtrl.Instance.Pause) return;
            PlayerSwingingLeft.IsTrySwing = false;
        };
        InputManager.Gameplay.Attack.started += function => { 
            if (GPCtrl.Instance.Pause) return;
            if (GPCtrl.Instance.DashPause) PlayerDash.Dash();
            else PlayerAttack.Attack(); 
        };
        InputManager.Gameplay.Jump.started += function => {
            if (GPCtrl.Instance.Pause) return;
            if (PlayerSwingingLeft.IsSwinging || PlayerSwingingRight.IsSwinging)
                PlayerGrappleBoost.Boost();
            else
                PlayerMovement.Jump();
        };
        InputManager.Gameplay.Menu.started += function => GPCtrl.Instance.UICtrl.CallPause();
        //InputManager.Gameplay.Lock.started += function => GPCtrl.Instance.CameraThirdPerson.ActivateFreeLook(!GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.enabled);
    }

    private void OnDisable()
    {
        InputManager.Disable();
    }
}
