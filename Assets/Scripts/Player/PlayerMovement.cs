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
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Player.Data.charaHeight * 0.5f + 0.3f, WhatIsGround))
        {
            Debug.Log("ground hit with : " + hit.transform.gameObject.name);
        }
        _grounded = Physics.Raycast(transform.position, Vector3.down, Player.Data.charaHeight * 0.5f + 0.3f, WhatIsGround);
        _horizontalInput = Player.InputManager.Gameplay.Move.ReadValue<Vector2>().x;
        _verticalInput = Player.InputManager.Gameplay.Move.ReadValue<Vector2>().y;
        if (Player.PlayerAttack.IsGrappling)
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
        else if (_grounded) CurrentMoveSpeed = Player.Data.walkSpeed;

        if (_grounded)
        {
            Player.Rigibody.drag = Player.Data.groundDrag;
            Player.Animator.SetBool("Grounded", true);
        }
        else
        {
            Player.Rigibody.drag = 0;
            Player.Animator.SetBool("Grounded", false);
        }

        //if (player.playerGrappling.isSwinging)
        //{
        //    player.playerMesh.transform.rotation = Quaternion.Slerp(GPCtrl.Instance.player.playerMesh.transform.rotation, Quaternion.LookRotation(rb.velocity), Time.deltaTime);
        //}
        if (_moveDirection != Vector3.zero && _grounded) Player.Animator.SetBool("isWalking", true);
        else if (_moveDirection == Vector3.zero && _grounded) Player.Animator.SetBool("isWalking", false);
        //else if (!_grounded) //jump animation

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
            Player.Animator.SetTrigger("Jump");
            Player.Animator.SetBool("Grounded", false);
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
