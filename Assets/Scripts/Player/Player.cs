using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerData Data;
    public PlayerControls InputManager;

    [Header("Player Scripts")]
    public PlayerMovement PlayerMovement;
    public PlayerSwinging PlayerSwingingRight;
    public PlayerSwinging PlayerSwingingLeft;
    public PlayerAttack PlayerAttack;

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
        //inputManager.Gameplay.LeftShoulder.started += function => { leftShoulder = true; };
        //inputManager.Gameplay.LeftShoulder.canceled += function => { leftShoulder = false; };
        //inputManager.Gameplay.RightShoulder.started += function => { rightShoulder = true; };
        //inputManager.Gameplay.RightShoulder.canceled += function => { rightShoulder = false; };
        InputManager.Gameplay.Jump.started += PlayerMovement.Jump;
    }

    private void OnDisable()
    {
        InputManager.Disable();
    }
}
