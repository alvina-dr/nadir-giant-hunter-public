using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Player player;
    [Header("Movement")]
    public float currentMoveSpeed;

    bool readyToJump;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;


    private void Start()
    {
        player.rigibody.freezeRotation = true;
        readyToJump = true;
    }

    private void Update()
    {
        // ground check
        //if (player.playerGrappling.isGrappling) return;
        grounded = Physics.Raycast(transform.position + new Vector3(0, 1, 0), Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);
        horizontalInput = player.inputManager.Gameplay.Move.ReadValue<Vector2>().x;
        verticalInput = player.inputManager.Gameplay.Move.ReadValue<Vector2>().y;
        SpeedControl();

        if (player.playerSwingingLeft.isSwinging || player.playerSwingingRight.isSwinging)
        {
            if (currentMoveSpeed < player.data.swingSpeed) currentMoveSpeed = player.data.swingSpeed;
            currentMoveSpeed += player.data.swingAcceleration * Time.deltaTime;
            if (currentMoveSpeed >= player.data.swingMaxSpeed) currentMoveSpeed = player.data.swingMaxSpeed;
        }
        else if (grounded) currentMoveSpeed = player.data.walkSpeed;

        if (grounded)
            player.rigibody.drag = player.data.groundDrag;
        else
            player.rigibody.drag = 0;

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
        moveDirection = player.orientation.forward * verticalInput + player.orientation.right * horizontalInput;

        // on ground
        if (grounded)
            player.rigibody.AddForce(moveDirection.normalized * currentMoveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            player.rigibody.AddForce(moveDirection.normalized * currentMoveSpeed * 10f * player.data.airMultiplier, ForceMode.Force);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (readyToJump && grounded)
        {
            readyToJump = false;
            player.rigibody.velocity = new Vector3(player.rigibody.velocity.x, 0f, player.rigibody.velocity.z);
            player.rigibody.AddForce(transform.up * player.data.jumpForce, ForceMode.Impulse);
            Invoke(nameof(ResetJump), player.data.jumpCooldown);
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(player.rigibody.velocity.x, 0f, player.rigibody.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > currentMoveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * currentMoveSpeed;
            player.rigibody.velocity = new Vector3(limitedVel.x, player.rigibody.velocity.y, limitedVel.z);
        }
    }
}
