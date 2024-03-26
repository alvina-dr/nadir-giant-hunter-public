using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerData Data;
    public PlayerVFXData VFXData;
    public PlayerControls InputManager;

    [Header("Player Scripts")]
    public PlayerMovement PlayerMovement;
    public PlayerSwinging PlayerSwingingRight;
    public PlayerSwinging PlayerSwingingLeft;
    public PlayerAttack PlayerAttack;
    public PlayerDoubleGrappleBoost PlayerDoubleGrappleBoost;

    [Header("Components")]
    public Transform Mesh;
    public Rigidbody Rigibody;
    public Transform Orientation;
    public Animator Animator;

    private void Awake()
    {
        InputManager = new PlayerControls();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        InputManager.Enable();
        InputManager.Gameplay.SwingRight.started += function => { PlayerSwingingRight.TrySwing = true; };
        InputManager.Gameplay.SwingRight.canceled += function => { PlayerSwingingRight.TrySwing = false; };
        InputManager.Gameplay.SwingLeft.started += function => { PlayerSwingingLeft.TrySwing = true; };
        InputManager.Gameplay.SwingLeft.canceled += function => { PlayerSwingingLeft.TrySwing = false; };
        InputManager.Gameplay.Attack.started += function => { PlayerAttack.Attack(); };
        InputManager.Gameplay.Jump.started += PlayerMovement.Jump;
    }

    private void OnDisable()
    {
        InputManager.Disable();
    }
}
