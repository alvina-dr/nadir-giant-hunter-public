using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

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
    [Sirenix.OdinInspector.ReadOnly]
    public float FallingTimer = 0.0f;

    //Debug
    [SerializeField, Sirenix.OdinInspector.ReadOnly]
    private float _CurrentSpeed;

    private void Start()
    {
        Player.Rigibody.freezeRotation = true;
        _readyToJump = true;
        Player.SoundData.SFX_Hunter_Movement_AirSpeed.Post(gameObject);
    }

    private void Update()
    {
        //speed effect
        Player.SparksVFX.SetVector3("Input Velocity", Player.Rigibody.velocity / Player.Data.speedDivisionFactorVFX);
        Material postProcess = GPCtrl.Instance.GetPostProcessMaterial();
        if (postProcess != null )
        {
            float speed = (Player.Rigibody.velocity.magnitude - Player.Data.speedEffectMin) / 100;
            if (speed < 0) speed = 0;
            float lerp = Mathf.Lerp(postProcess.GetFloat("_bypass_Input_Velocity_Factor"), speed, 0.1f);
            postProcess.SetFloat("_bypass_Input_Velocity_Factor", lerp);
            postProcess.SetVector("_Input_Velocity", Player.Rigibody.velocity / Player.Data.speedDivisionFactorVFX);
        }

        AkSoundEngine.SetRTPCValue("RTPC_Speed", _CurrentSpeed);
        AkSoundEngine.SetRTPCValue("RTPC_Depth", transform.position.y);

        if (transform.position.y < GPCtrl.Instance.GeneralData.yHeightPitBottom)
        {
            GPCtrl.Instance.UICtrl.PlayerLowIndicator.alpha = 1;
        } else
        {
            GPCtrl.Instance.UICtrl.PlayerLowIndicator.alpha = 0;
        }

        Grounded = Physics.Raycast(transform.position, Vector3.down, Player.Data.charaHeight * 0.5f + 0.3f, WhatIsGround);
        _horizontalInput = Player.PlayerInput.actions["Move"].ReadValue<Vector2>().x;
        _verticalInput = Player.PlayerInput.actions["Move"].ReadValue<Vector2>().y;
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
                CurrentMoveSpeed += Player.Data.swingAcceleration * Time.fixedDeltaTime;
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
            FallingTimer += Time.deltaTime;
            if (FallingTimer > Player.Data.timeBeforeLookingDownAnim)
            {
                Player.Animator.SetBool("LongFall", true);
            }
        } else
        {
            FallingTimer = 0;
            Player.Animator.SetBool("LongFall", false);
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (Player.PlayerAttack.IsGrappling || GPCtrl.Instance.Pause)
            return;
        if (Player.PlayerSwingingLeft.IsSwinging || Player.PlayerSwingingRight.IsSwinging)
        {
            Player.Rigibody.AddForce(Player.Orientation.right * _horizontalInput * Player.Data.StrafeInfluence, ForceMode.Force);
            return;
        }
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
            Vector3 input = new Vector3(Player.MoveAction.ReadValue<Vector2>().x, 0, Player.MoveAction.ReadValue<Vector2>().y);
            if (input != Vector3.zero && !Grounded)
            {
                if (input.z > 0 && Camera.main.transform.forward.y < -.3f)
                {
                    Player.Rigibody.AddForce(Vector3.down * Player.Data.jumpForce, ForceMode.Impulse);
                } else
                {
                    Player.Rigibody.AddForce((input + transform.up).normalized * Player.Data.jumpForce, ForceMode.Impulse);
                }
            } else
            {
                Player.Rigibody.AddForce(transform.up * Player.Data.jumpForce, ForceMode.Impulse);
            }
            Invoke(nameof(ResetJump), Player.Data.jumpCooldown);
            Player.Animator.SetBool("Grounded", false);
            Player.Animator.SetFloat("JumpDirectionX", Mathf.Round(input.x));
            Player.Animator.SetFloat("JumpDirectionY", input.z);
            Player.Animator.SetTrigger("Jump");
            Player.SoundData.SFX_Hunter_Jump.Post(Player.gameObject);
            Player.SparksVFX.SendEvent("Jump");
            DataHolder.Instance.RumbleManager.PulseFor(5f, 5f, .1f);
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
        if (Player.PlayerDash.IsDashing || Player.PlayerAttack.IsGrappling || GPCtrl.Instance.Pause) return;
        if (!Player.PlayerSwingingLeft.IsSwinging && !Player.PlayerSwingingRight.IsSwinging)
        {
            if (Grounded) return;
            Vector3 flatVel = new Vector3(Player.Rigibody.velocity.x, 0f, Player.Rigibody.velocity.z);
            if (flatVel.magnitude > Player.Data.maxSpeedInAir) // limit velocity if needed
            {
                Vector3 limitedVel = flatVel.normalized * Player.Data.maxSpeedInAir * Time.fixedDeltaTime * 60;
                limitedVel = Vector3.Lerp(Player.Rigibody.velocity, limitedVel, Player.Data.maxSpeedInAirLerp * (1+Time.fixedDeltaTime));
                Player.Rigibody.velocity = new Vector3(limitedVel.x, Player.Rigibody.velocity.y, limitedVel.z);
            }

        }
        else
        {
            Vector3 flatVel = new Vector3(Player.Rigibody.velocity.x, 0f, Player.Rigibody.velocity.z);
            if (flatVel.magnitude > CurrentMoveSpeed) // limit velocity if needed
            {
                Vector3 limitedVel = flatVel.normalized * CurrentMoveSpeed * Time.fixedDeltaTime * 60;
                //Debug.Log(limitedVel.x + limitedVel.z);
                Player.Rigibody.velocity = new Vector3(limitedVel.x, Player.Rigibody.velocity.y, limitedVel.z);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //WALL JUMP
        if (!Grounded && collision.contacts[0].normal.y < 0.1f && collision.contacts[0].normal.y > -0.5f)
        {
            //Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal * 5, Color.red, 15f);
            WallJump(collision.contacts[0].normal);
        }
    }

    public void WallJump(Vector3 wallNormal)
    {
        CanJumpOnceInAir = true;
        Player.Rigibody.velocity = new Vector3(Player.Rigibody.velocity.x, Mathf.Max(Player.Rigibody.velocity.y, 0), Player.Rigibody.velocity.z);
        Player.Rigibody.AddForce(transform.up * Player.Data.jumpForce + wallNormal * Player.Data.jumpForce, ForceMode.Impulse);
        Player.SoundData.SFX_Hunter_Jump.Post(Player.gameObject);
        Player.SparksVFX.SendEvent("Jump");
        Player.Animator.SetBool("LongFall", false);
        Player.Animator.SetFloat("JumpDirectionX", Player.Rigibody.velocity.x);
        Player.Animator.SetFloat("JumpDirectionY", Player.Rigibody.velocity.z);
        Player.Animator.SetTrigger("Jump");
        DataHolder.Instance.RumbleManager.PulseFor(5f, 5f, .1f);
    }
}