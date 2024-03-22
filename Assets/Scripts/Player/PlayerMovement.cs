using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Player Player;
    [Header("Movement")]
    public float CurrentMoveSpeed;

    [Header("Ground Check")]
    public float PlayerHeight;
    public LayerMask WhatIsGround;

    private bool _grounded;
    private bool _readyToJump;
    private float _horizontalInput;
    private float _verticalInput;
    private Vector3 _moveDirection;


    private void Start()
    {
        Player.Rigibody.freezeRotation = true;
        _readyToJump = true;
    }

    private void Update()
    {
        _grounded = Physics.Raycast(transform.position + new Vector3(0, 1, 0), Vector3.down, PlayerHeight * 0.5f + 0.3f, WhatIsGround);
        _horizontalInput = Player.InputManager.Gameplay.Move.ReadValue<Vector2>().x;
        _verticalInput = Player.InputManager.Gameplay.Move.ReadValue<Vector2>().y;
        SpeedControl();

        if (Player.PlayerSwingingLeft.IsSwinging || Player.PlayerSwingingRight.IsSwinging)
        {
            if (CurrentMoveSpeed < Player.Data.swingSpeed) CurrentMoveSpeed = Player.Data.swingSpeed;
            CurrentMoveSpeed += Player.Data.swingAcceleration * Time.deltaTime;
            if (CurrentMoveSpeed >= Player.Data.swingMaxSpeed) CurrentMoveSpeed = Player.Data.swingMaxSpeed;
        }
        else if (_grounded) CurrentMoveSpeed = Player.Data.walkSpeed;

        if (_grounded)
            Player.Rigibody.drag = Player.Data.groundDrag;
        else
            Player.Rigibody.drag = 0;

        //if (player.playerGrappling.isSwinging)
        //{
        //    player.playerMesh.transform.rotation = Quaternion.Slerp(GPCtrl.Instance.player.playerMesh.transform.rotation, Quaternion.LookRotation(rb.velocity), Time.deltaTime);
        //}


    }

    private void FixedUpdate()
    {
        //if (player.playerGrappling.isGrappling) return;
        MovePlayer();
    }

    private void MovePlayer()
    {
        // calculate movement direction
        _moveDirection = Player.Orientation.forward * _verticalInput + Player.Orientation.right * _horizontalInput;

        
        if (_grounded) // on ground
            Player.Rigibody.AddForce(_moveDirection.normalized * CurrentMoveSpeed * 10f, ForceMode.Force);
        else // in air
            Player.Rigibody.AddForce(_moveDirection.normalized * CurrentMoveSpeed * 10f * Player.Data.airMultiplier, ForceMode.Force);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (_readyToJump && _grounded)
        {
            _readyToJump = false;
            Player.Rigibody.velocity = new Vector3(Player.Rigibody.velocity.x, 0f, Player.Rigibody.velocity.z);
            Player.Rigibody.AddForce(transform.up * Player.Data.jumpForce, ForceMode.Impulse);
            Invoke(nameof(ResetJump), Player.Data.jumpCooldown);
        }
    }

    private void ResetJump()
    {
        _readyToJump = true;
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(Player.Rigibody.velocity.x, 0f, Player.Rigibody.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > CurrentMoveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * CurrentMoveSpeed;
            Player.Rigibody.velocity = new Vector3(limitedVel.x, Player.Rigibody.velocity.y, limitedVel.z);
        }
    }
}
