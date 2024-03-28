using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraThirdPerson : MonoBehaviour
{
    public Vector3 LookDirectionSave = Vector3.zero;

    private void LateUpdate()
    {
        Vector3 viewDir = GPCtrl.Instance.Player.transform.position - new Vector3(transform.position.x, GPCtrl.Instance.Player.transform.position.y, transform.position.z);
        GPCtrl.Instance.Player.Orientation.forward = viewDir.normalized;

        float inputHorizontal = GPCtrl.Instance.Player.InputManager.Gameplay.Move.ReadValue<Vector2>().x;
        float inputVertical = GPCtrl.Instance.Player.InputManager.Gameplay.Move.ReadValue<Vector2>().y;
        Vector3 inputDir = GPCtrl.Instance.Player.Orientation.forward * inputVertical + GPCtrl.Instance.Player.Orientation.right * inputHorizontal;
        if (inputDir != Vector3.zero && !GPCtrl.Instance.Player.PlayerAttack.IsGrappling) {
            Vector3 upVector = Vector3.up;
            if (GPCtrl.Instance.Player.PlayerSwingingLeft.IsSwinging || GPCtrl.Instance.Player.PlayerSwingingRight.IsSwinging)
            {
                inputDir = Camera.main.transform.forward;
                upVector = GPCtrl.Instance.Player.Mesh.up;
            }
            LookDirectionSave = inputDir;
            //float rotationSpeed = GPCtrl.Instance.Player.Data.walkRotationSpeed;
            //if (GPCtrl.Instance.Player.PlayerSwingingLeft.IsSwinging && GPCtrl.Instance.Player.PlayerSwingingRight.IsSwinging)
            //    rotationSpeed = GPCtrl.Instance.Player.Data.airRotationSpeed;
            GPCtrl.Instance.Player.Mesh.rotation = Quaternion.LookRotation(inputDir, upVector);
        }
        else if (inputDir == Vector3.zero && !GPCtrl.Instance.Player.PlayerAttack.IsGrappling)
        {
            if (GPCtrl.Instance.Player.PlayerSwingingLeft.IsSwinging || GPCtrl.Instance.Player.PlayerSwingingRight.IsSwinging)
            {
                inputDir = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
                LookDirectionSave = inputDir;
            }
            else inputDir = LookDirectionSave;
            //float rotationSpeed = GPCtrl.Instance.Player.Data.walkRotationSpeed;
            //if (GPCtrl.Instance.Player.PlayerSwingingLeft.IsSwinging && GPCtrl.Instance.Player.PlayerSwingingRight.IsSwinging)
            //    rotationSpeed = GPCtrl.Instance.Player.Data.airRotationSpeed;
            if (inputDir != Vector3.zero) 
                GPCtrl.Instance.Player.Mesh.rotation = Quaternion.LookRotation(inputDir, GPCtrl.Instance.Player.Mesh.up);
        } else if (GPCtrl.Instance.Player.PlayerAttack.IsGrappling)
        {
            Vector3 direction = (GPCtrl.Instance.Player.PlayerAttack.CurrentWeakSpot.transform.position - GPCtrl.Instance.Player.transform.transform.position).normalized;
            GPCtrl.Instance.Player.Mesh.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z), Vector3.up);
            LookDirectionSave = direction;
        }
    }
}
