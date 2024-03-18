using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerData data;
    public PlayerControls inputManager;

    [Header("Player Scripts")]
    public PlayerMovement playerMovement;
    public PlayerSwinging playerSwingingRight;
    public PlayerSwinging playerSwingingLeft;

    [Header("Components")]
    public Transform mesh;
    public Rigidbody rigibody;
    public Transform orientation;

    private void Awake()
    {
        inputManager = new PlayerControls();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        inputManager.Enable();
        inputManager.Gameplay.SwingRight.started += function => { playerSwingingRight.trySwing = true; };
        inputManager.Gameplay.SwingRight.canceled += function => { playerSwingingRight.trySwing = false; };
        inputManager.Gameplay.SwingLeft.started += function => { playerSwingingLeft.trySwing = true; };
        inputManager.Gameplay.SwingLeft.canceled += function => { playerSwingingLeft.trySwing = false; };
        //inputManager.Gameplay.LeftShoulder.started += function => { leftShoulder = true; };
        //inputManager.Gameplay.LeftShoulder.canceled += function => { leftShoulder = false; };
        //inputManager.Gameplay.RightShoulder.started += function => { rightShoulder = true; };
        //inputManager.Gameplay.RightShoulder.canceled += function => { rightShoulder = false; };
        inputManager.Gameplay.Jump.started += playerMovement.Jump;
    }

    private void OnDisable()
    {
        inputManager.Disable();
    }
}
