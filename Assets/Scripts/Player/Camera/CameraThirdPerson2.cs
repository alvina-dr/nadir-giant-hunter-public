using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraThirdPerson2 : MonoBehaviour
{
    public CinemachineVirtualCamera CinemachineVirtual;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 viewDir = GPCtrl.Instance.Player.transform.position - new Vector3(transform.position.x, GPCtrl.Instance.Player.transform.position.y, transform.position.z);
        GPCtrl.Instance.Player.Orientation.forward = viewDir.normalized;

    }
}
