using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class PlayerClimbMovementTEMP : MonoBehaviour
{
    public float speed = 1;
    public float jumpForce = 1;
    public float drag = 1;

    [ReadOnly, SerializeField] private float currentDrag = 0;

    private Camera _camera;
    private PlayerInputsTemp _playerInput;
    private Rigidbody _rigidbody;
    private Climbing _climbing;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _camera = Camera.main;
        _playerInput = new PlayerInputsTemp();
        _playerInput.Enable();
        _playerInput.InGame.Jump.performed += Jump;
        _rigidbody = GetComponent<Rigidbody>();
        _climbing = GetComponent<Climbing>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (_playerInput.InGame.Move.IsInProgress())
        {
            if (_climbing.IsOnMesh())
            {
                _climbing.MoveToward(Camera.main.transform.right, Camera.main.transform.forward, speed/100 * Time.fixedDeltaTime);
                return;
            }
            Vector2 dir = _playerInput.InGame.Move.ReadValue<Vector2>();
            Vector3 force = new Vector3(_camera.transform.forward.x, 0, _camera.transform.forward.z) * dir.y * speed + _camera.transform.right * dir.x * speed;
            _rigidbody.AddForce(force);
        }
        currentDrag = Mathf.Max(drag * Time.fixedDeltaTime * 100, 1);
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x / currentDrag, _rigidbody.velocity.y, _rigidbody.velocity.z / currentDrag);
    }

    private void Jump(InputAction.CallbackContext callbackContext)
    {
        if (_climbing.IsOnMesh())
        {
            _climbing.StopMeshClimbing();
        }
        _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
