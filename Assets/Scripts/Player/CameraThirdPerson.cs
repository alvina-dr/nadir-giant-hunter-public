using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraThirdPerson : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float rotationSpeed;

    private void Update()
    {
        Vector3 _viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        GPCtrl.Instance.Player.Orientation.forward = _viewDir.normalized;

        float _inputHorizontal = Input.GetAxisRaw("Horizontal");
        float _inputVertical = Input.GetAxisRaw("Vertical");
        Vector3 _inputDir = GPCtrl.Instance.Player.Orientation.forward * _inputVertical + GPCtrl.Instance.Player.Orientation.right * _inputHorizontal;
        if (_inputDir != Vector3.zero && !GPCtrl.Instance.Player.PlayerAttack.IsGrappling) {
            GPCtrl.Instance.Player.Mesh.forward = Vector3.Slerp(GPCtrl.Instance.Player.Mesh.forward, _inputDir.normalized, Time.deltaTime * rotationSpeed);
        }    
    }
}
