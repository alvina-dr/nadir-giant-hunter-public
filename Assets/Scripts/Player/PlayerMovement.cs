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

    public bool Grounded;
    private bool _readyToJump;
    private float _horizontalInput;
    private float _verticalInput;
    private Vector3 _moveDirection;
    public bool CanJumpOnceInAir;

    private void Start()
    {
        Player.Rigibody.freezeRotation = true;
        _readyToJump = true;
    }

    private void Update()
    {
        Grounded = Physics.Raycast(transform.position, Vector3.down, Player.Data.charaHeight * 0.5f + 0.3f, WhatIsGround);
        _horizontalInput = Player.InputManager.Gameplay.Move.ReadValue<Vector2>().x;
        _verticalInput = Player.InputManager.Gameplay.Move.ReadValue<Vector2>().y;
        if (Player.PlayerAttack.IsGrappling || GPCtrl.Instance.Pause || GPCtrl.Instance.DashPause)
        {
            _horizontalInput = 0;
            _verticalInput = 0;
        }
        SpeedControl();

        if (Player.PlayerSwingingLeft.IsSwinging || Player.PlayerSwingingRight.IsSwinging)
        {
            if (CurrentMoveSpeed < Player.Data.swingSpeed) CurrentMoveSpeed = Player.Data.swingSpeed;
            CurrentMoveSpeed += Player.Data.swingAcceleration * Time.deltaTime;
            if (CurrentMoveSpeed >= Player.Data.swingMaxSpeed) CurrentMoveSpeed = Player.Data.swingMaxSpeed;
        }
        else if (Grounded) CurrentMoveSpeed = Player.Data.walkSpeed;

        if (Grounded)
        {
            Player.Rigibody.drag = Player.Data.groundDrag;
            Player.Animator.SetBool("Grounded", true);
            CanJumpOnceInAir = true;
        }
        else
        {
            Player.Rigibody.drag = 0;
            Player.Animator.SetBool("Grounded", false);
        }

        if (_moveDirection != Vector3.zero && Grounded)
            Player.Animator.SetBool("isWalking", true);
        else if (_moveDirection == Vector3.zero && Grounded) Player.Animator.SetBool("isWalking", false);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        _moveDirection = Player.Orientation.forward * _verticalInput + Player.Orientation.right * _horizontalInput; // calculate movement direction

        if (Grounded) // on ground
            Player.Rigibody.AddForce(_moveDirection.normalized * CurrentMoveSpeed * 10f, ForceMode.Force);
        else // in air
            Player.Rigibody.AddForce(_moveDirection.normalized * CurrentMoveSpeed * 10f * Player.Data.airMultiplier, ForceMode.Force);
    }

    public void Jump()
    {
        if (_readyToJump && CanJumpOnceInAir)
        {
            CanJumpOnceInAir = false;
            _readyToJump = false;
            Player.Rigibody.velocity = new Vector3(Player.Rigibody.velocity.x, 0f, Player.Rigibody.velocity.z);
            Player.Rigibody.AddForce(transform.up * Player.Data.jumpForce, ForceMode.Impulse);
            Invoke(nameof(ResetJump), Player.Data.jumpCooldown);
            Player.Animator.SetTrigger("Jump");
            Player.Animator.SetBool("Grounded", false);
            Player.SoundData.SFX_Hunter_Jump.Post(Player.gameObject);
        }
    }

    private void ResetJump()
    {
        _readyToJump = true;
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(Player.Rigibody.velocity.x, 0f, Player.Rigibody.velocity.z);
        if (flatVel.magnitude > CurrentMoveSpeed) // limit velocity if needed
        {
            Vector3 limitedVel = flatVel.normalized * CurrentMoveSpeed;
            Player.Rigibody.velocity = new Vector3(limitedVel.x, Player.Rigibody.velocity.y, limitedVel.z);
        }
    }
}
