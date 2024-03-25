using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraThirdPerson : MonoBehaviour
{
    private void Update()
    {
        Vector3 viewDir = GPCtrl.Instance.Player.transform.position - new Vector3(transform.position.x, GPCtrl.Instance.Player.transform.position.y, transform.position.z);
        GPCtrl.Instance.Player.Orientation.forward = viewDir.normalized;

        float inputHorizontal = GPCtrl.Instance.Player.InputManager.Gameplay.Move.ReadValue<Vector2>().x;
        float inputVertical = GPCtrl.Instance.Player.InputManager.Gameplay.Move.ReadValue<Vector2>().y;
        Vector3 inputDir = GPCtrl.Instance.Player.Orientation.forward * inputVertical + GPCtrl.Instance.Player.Orientation.right * inputHorizontal;
        if (inputDir != Vector3.zero && !GPCtrl.Instance.Player.PlayerAttack.IsGrappling) {
            if (GPCtrl.Instance.Player.PlayerSwingingLeft.IsSwinging && !GPCtrl.Instance.Player.PlayerSwingingRight.IsSwinging) 
            {
                inputDir = new Vector3(Camera.main.transform.forward.x, Camera.main.transform.forward.y, Camera.main.transform.forward.z);
            }
            if (!GPCtrl.Instance.Player.PlayerSwingingLeft.IsSwinging && GPCtrl.Instance.Player.PlayerSwingingRight.IsSwinging)
            {
                inputDir = new Vector3(Camera.main.transform.forward.x, Camera.main.transform.forward.y, Camera.main.transform.forward.z);
            }
            float rotationSpeed = GPCtrl.Instance.Player.Data.walkRotationSpeed;
            if (GPCtrl.Instance.Player.PlayerSwingingLeft.IsSwinging && GPCtrl.Instance.Player.PlayerSwingingRight.IsSwinging)
                rotationSpeed = GPCtrl.Instance.Player.Data.airRotationSpeed;
            GPCtrl.Instance.Player.Mesh.forward = Vector3.Slerp(GPCtrl.Instance.Player.Mesh.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }    
    }
}
