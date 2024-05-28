using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Player Player;
    [Header("Movement")]
    public float CurrentMoveSpeed;

    [Header("Ground Check")]
    public float PlayerHeight;
    public LayerMask WhatIsGround;

    [Sirenix.OdinInspector.ReadOnly]
    public bool Grounded;
    private bool _readyToJump;
    private float _horizontalInput;
    private float _verticalInput;
    private Vector3 _moveDirection;
    [Sirenix.OdinInspector.ReadOnly]
    public bool CanJumpOnceInAir;
    private float _fallingTimer = 0.0f;

    //Debug
    [SerializeField, Sirenix.OdinInspector.ReadOnly]
    private float _CurrentSpeed;

    private void Start()
    {
        Player.Rigibody.freezeRotation = true;
        _readyToJump = true;
    }

    private void Update()
    {
        //speed effect
        Player.SparksVFX.SetVector3("Input Velocity", Player.Rigibody.velocity / Player.Data.speedDivisionFactorVFX);
        Material postProcess = GPCtrl.Instance.GetPostProcessMaterial();
        if (postProcess != null )
        {
            float speed = (Player.Rigibody.velocity.magnitude - 150) / 100;
            if (speed < 0) speed = 0;
            float lerp = Mathf.Lerp(postProcess.GetFloat("_bypass_Input_Velocity_Factor"), speed, 0.1f);
            postProcess.SetFloat("_bypass_Input_Velocity_Factor", lerp);
            postProcess.SetVector("_Input_Velocity", Player.Rigibody.velocity / Player.Data.speedDivisionFactorVFX);
        }

        if (transform.position.y < GPCtrl.Instance.GeneralData.yHeightPitBottom)
        {
            GPCtrl.Instance.UICtrl.PlayerLowIndicator.alpha = 1;
        } else
        {
            GPCtrl.Instance.UICtrl.PlayerLowIndicator.alpha = 0;
        }

        Grounded = Physics.Raycast(transform.position, Vector3.down, Player.Data.charaHeight * 0.5f + 0.3f, WhatIsGround);
        _horizontalInput = Player.InputManager.Gameplay.Move.ReadValue<Vector2>().x;
        _verticalInput = Player.InputManager.Gameplay.Move.ReadValue<Vector2>().y;
        if (Player.PlayerAttack.IsGrappling || Player.PlayerDash.IsDashing || GPCtrl.Instance.Pause || GPCtrl.Instance.DashPause)
        {
            _horizontalInput = 0;
            _verticalInput = 0;
        }
        SpeedControl();

        if (Player.PlayerSwingingLeft.IsSwinging || Player.PlayerSwingingRight.IsSwinging)
        {
            if (!GPCtrl.Instance.Pause)
            {
                //Keep move speed btwn min and max of swing speed AND add an acceleration to it.
                CurrentMoveSpeed = Mathf.Max(Player.Data.swingSpeed, CurrentMoveSpeed);
                CurrentMoveSpeed += Player.Data.swingAcceleration * Time.deltaTime;
                CurrentMoveSpeed = Mathf.Min(Player.Data.swingMaxSpeed, CurrentMoveSpeed);
            }
        }
        else if (Grounded) CurrentMoveSpeed = Player.Data.walkSpeed;//set moveSpeed to WalkSpeed when grounded

        _CurrentSpeed = Player.Rigibody.velocity.magnitude;

        //Set drag depending on groundState and animation
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

        //walk Animation
        if (_moveDirection != Vector3.zero && Grounded)
            Player.Animator.SetBool("isWalking", true);
        else if (_moveDirection == Vector3.zero && Grounded)
            Player.Animator.SetBool("isWalking", false);

        if (!GPCtrl.Instance.Pause && !Grounded && !Player.PlayerSwingingLeft.IsSwinging && !Player.PlayerSwingingRight.IsSwinging && !Player.PlayerDash.IsDashing && !Player.PlayerAttack.IsGrappling && Player.Rigibody.velocity.y < -5)
        {
            _fallingTimer += Time.deltaTime;
            if (_fallingTimer > Player.Data.timeBeforeLookingDownAnim)
            {
                Player.Animator.SetBool("LongFall", true);
            }
        } else
        {
            _fallingTimer = 0;
            Player.Animator.SetBool("LongFall", false);
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (Player.PlayerAttack.IsGrappling) return;
        _moveDirection = Player.Orientation.forward * _verticalInput + Player.Orientation.right * _horizontalInput; // calculate input movement direction

        //add force from input and player velo with certain force (air control when in air)
        if (Grounded) // on ground
            Player.Rigibody.AddForce(_moveDirection.normalized * CurrentMoveSpeed * 10f, ForceMode.Force);
        else // in air
        {
            
            Player.Rigibody.AddForce(_moveDirection.normalized * CurrentMoveSpeed * 10f * Player.Data.airMultiplier, ForceMode.Force);
        }
    }

    public void Jump()
    {
        if (_readyToJump && CanJumpOnceInAir && !Player.PlayerAttack.IsGrappling)
        {
            CanJumpOnceInAir = false;
            _readyToJump = false;
            Player.Rigibody.velocity = new Vector3(Player.Rigibody.velocity.x, Mathf.Max(Player.Rigibody.velocity.y, 0), Player.Rigibody.velocity.z);
            Player.Rigibody.AddForce(transform.up * Player.Data.jumpForce, ForceMode.Impulse);
            Invoke(nameof(ResetJump), Player.Data.jumpCooldown);
            Player.Animator.SetTrigger("Jump");
            Player.Animator.SetBool("Grounded", false);
            Player.SoundData.SFX_Hunter_Jump.Post(Player.gameObject);
            Player.SparksVFX.SendEvent("Jump");
        }
    }

    private void ResetJump()
    {
        _readyToJump = true;
    }

    /// <summary>
    /// Clamp the speed of the player when is not dashing to the currentMoveSpeed value
    /// </summary>
    private void SpeedControl()
    {
        if (Player.PlayerDash.IsDashing || Player.PlayerAttack.IsGrappling) return;
        //if (!Player.PlayerSwingingLeft.IsSwinging && !Player.PlayerSwingingRight.IsSwinging)
        //{
        //    //Vector3 flatVel = new Vector3(Player.Rigibody.velocity.x, 0f, Player.Rigibody.velocity.z);
        //    //if (flatVel.magnitude > Player.Data.maxSpeedInAir) // limit velocity if needed
        //    //{
        //    //    Vector3 limitedVel = flatVel.normalized * Player.Data.maxSpeedInAir * Time.deltaTime * 60;
        //    //    //Debug.Log(limitedVel.x + limitedVel.z);
        //    //    Player.Rigibody.velocity = new Vector3(limitedVel.x, Player.Rigibody.velocity.y, limitedVel.z);
        //    //}

        //} else
        {
            Vector3 flatVel = new Vector3(Player.Rigibody.velocity.x, 0f, Player.Rigibody.velocity.z);
            if (flatVel.magnitude > CurrentMoveSpeed) // limit velocity if needed
            {
                Vector3 limitedVel = flatVel.normalized * CurrentMoveSpeed * Time.deltaTime * 60;
                //Debug.Log(limitedVel.x + limitedVel.z);
                Player.Rigibody.velocity = new Vector3(limitedVel.x, Player.Rigibody.velocity.y, limitedVel.z);
            }
        }
    }
}
