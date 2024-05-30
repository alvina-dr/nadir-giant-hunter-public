using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraThirdPerson : MonoBehaviour
{
    public Vector3 LookDirectionSave = Vector3.zero;
    public CinemachineFreeLook CinemachineFreeLook;
    public CinemachineTargetGroup CinemachineTargetGroup;
    public CameraShake CameraShake;
    public CinemachineInputProvider InputProvider;
    //public bool canMoveCamera;

    private void Update()
    {
        //if (!canMoveCamera) return;
        if (GPCtrl.Instance.Pause) return;
        //camera mobility depending on grounded or not (can see higher up from below if not on ground, but need to see less high if grounded to avoid clipping with ground)
        if (GPCtrl.Instance.Player.PlayerMovement.Grounded)
        {
            GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.m_Orbits[2].m_Height = -1;
        } else
        {
            GPCtrl.Instance.CameraThirdPerson.CinemachineFreeLook.m_Orbits[2].m_Height = -4;
        }

        //CAM SENSI
        if (GPCtrl.Instance.DashPause) return;
        float camSensi = 1;
        if (PlayerPrefs.HasKey("CamSensi")) camSensi = PlayerPrefs.GetFloat("CamSensi");
        CinemachineFreeLook.m_XAxis.m_MaxSpeed = camSensi * 300;
        CinemachineFreeLook.m_YAxis.m_MaxSpeed = camSensi * 2;


        Vector3 viewDir = GPCtrl.Instance.Player.transform.position - new Vector3(transform.position.x, GPCtrl.Instance.Player.transform.position.y, transform.position.z);
        GPCtrl.Instance.Player.Orientation.forward = viewDir.normalized;

        float inputHorizontal = GPCtrl.Instance.Player.InputManager.Gameplay.Move.ReadValue<Vector2>().x;
        float inputVertical = GPCtrl.Instance.Player.InputManager.Gameplay.Move.ReadValue<Vector2>().y;
        Vector3 inputDir = GPCtrl.Instance.Player.Orientation.forward * inputVertical + GPCtrl.Instance.Player.Orientation.right * inputHorizontal;

        //isn't attack grappling and is moving
        if (inputDir != Vector3.zero && !GPCtrl.Instance.Player.PlayerAttack.IsGrappling) {
            //setup up vector
            Vector3 upVector = Vector3.up;
            if (GPCtrl.Instance.Player.PlayerSwingingLeft.IsSwinging || GPCtrl.Instance.Player.PlayerSwingingRight.IsSwinging)
            {
                inputDir = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
                upVector = GPCtrl.Instance.Player.Mesh.up;
            }

            //Pause dir
            if (GPCtrl.Instance.Pause) inputDir = LookDirectionSave;
            LookDirectionSave = inputDir;

            GPCtrl.Instance.Player.PlayerSwingingLeft.CalculateUpVector();
            GPCtrl.Instance.Player.PlayerSwingingRight.CalculateUpVector();

            //is swinging
            if (GPCtrl.Instance.Player.PlayerSwingingLeft.IsSwinging || GPCtrl.Instance.Player.PlayerSwingingRight.IsSwinging || !GPCtrl.Instance.Player.PlayerMovement.Grounded)
            {
                /*Vector3 orientation = new Vector3(GPCtrl.Instance.Player.Rigibody.velocity.x, 0, GPCtrl.Instance.Player.Rigibody.velocity.z);
                GPCtrl.Instance.Player.Mesh.rotation = Quaternion.LookRotation(orientation, upVector);*/

                Vector3 SpeedDir = new Vector3(GPCtrl.Instance.Player.Rigibody.velocity.x, 0, GPCtrl.Instance.Player.Rigibody.velocity.z);
                GPCtrl.Instance.Player.Mesh.rotation = Quaternion.LookRotation(SpeedDir, GPCtrl.Instance.Player.Mesh.up);

                GPCtrl.Instance.Player.PlayerSwingingLeft.SwingInfluenceDirection = inputDir;
                GPCtrl.Instance.Player.PlayerSwingingRight.SwingInfluenceDirection = inputDir;

            }
            else
            {
                float rotationSpeed = GPCtrl.Instance.Player.Data.walkRotationSpeed;
                GPCtrl.Instance.Player.Mesh.rotation = Quaternion.Lerp(GPCtrl.Instance.Player.Mesh.rotation, Quaternion.LookRotation(inputDir, upVector), rotationSpeed * Time.deltaTime);
            }

            if (!GPCtrl.Instance.Player.PlayerSwingingLeft.IsSwinging && !GPCtrl.Instance.Player.PlayerSwingingRight.IsSwinging) // if no swinging
            {
                inputDir = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
                GPCtrl.Instance.Player.Mesh.rotation = Quaternion.LookRotation(inputDir, upVector);
            }
        }
        else if (inputDir == Vector3.zero && !GPCtrl.Instance.Player.PlayerAttack.IsGrappling)//isn't attack grappling and isn't moving
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
            GPCtrl.Instance.Player.PlayerSwingingLeft.CalculateUpVector();
            GPCtrl.Instance.Player.PlayerSwingingRight.CalculateUpVector();
            if (inputDir != Vector3.zero) 
                GPCtrl.Instance.Player.Mesh.rotation = Quaternion.LookRotation(inputDir, GPCtrl.Instance.Player.Mesh.up);
        } 
        else if (GPCtrl.Instance.Player.PlayerAttack.IsGrappling) // is attack grappling
        {
            Vector3 direction = (GPCtrl.Instance.Player.PlayerAttack.CurrentTargetSpot.transform.position - GPCtrl.Instance.Player.transform.transform.position).normalized;
            GPCtrl.Instance.Player.Mesh.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z), Vector3.up);
            LookDirectionSave = direction;
        }
    }
}
