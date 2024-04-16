using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public PlayerGrappleBoost PlayerDoubleGrappleBoost;

    [Header("Components")]
    public Transform Mesh;
    public Rigidbody Rigibody;
    public Transform Orientation;
    public Animator Animator;

    [Header("Variables")]
    public float currentTimerPitBottom;
    public AK.Wwise.Event hitEvent;

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
            if (!PlayerDoubleGrappleBoost.IsGrapplingBoost)
                PlayerSwingingRight.TrySwing = true; 
        };
        InputManager.Gameplay.SwingRight.canceled += function => {
            if (GPCtrl.Instance.Pause) return;
            PlayerSwingingRight.TrySwing = false;
        };
        InputManager.Gameplay.SwingLeft.started += function => {
            if (GPCtrl.Instance.Pause) return;
            if (!PlayerDoubleGrappleBoost.IsGrapplingBoost)
                PlayerSwingingLeft.TrySwing = true; 
        };
        InputManager.Gameplay.SwingLeft.canceled += function => {
            if (GPCtrl.Instance.Pause) return;
            PlayerSwingingLeft.TrySwing = false;
        };
        InputManager.Gameplay.Attack.started += function => { 
            if (GPCtrl.Instance.Pause) return;
            PlayerAttack.Attack(); 
        };
        InputManager.Gameplay.Jump.started += function => {
            if (GPCtrl.Instance.Pause) return;
            if (PlayerSwingingLeft.IsSwinging || PlayerSwingingRight.IsSwinging)
                PlayerDoubleGrappleBoost.Boost();
            else
                PlayerMovement.Jump();
        };
        InputManager.Gameplay.Menu.started += function => GPCtrl.Instance.UICtrl.CallPause();
    }

    private void OnDisable()
    {
        InputManager.Disable();
    }

}
