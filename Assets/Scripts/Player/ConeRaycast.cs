using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeRaycast : MonoBehaviour
{
    public List<Vector3> contactPointList = new List<Vector3>();
    [SerializeField] private OrientationMode orientationMode;
    public Transform perfectPoint;
    enum OrientationMode
    {
        Y = 0,
        X = 1,
        Z = 2
    }

    public LayerMask layerMask;

    [Header("RAYCAST METHOD")]
    public bool SearchPoint = false;
    public float MinRadius;
    public float MaxRadius;
    [SerializeField] private int _anglePrecision;
    [SerializeField] int _radiusPrecision;
    private int frameCounter;

    private void Update()
    {
        frameCounter++;
        if (frameCounter == _radiusPrecision) {
            contactPointList.Clear();
            frameCounter = 0;
        }
        //Debug.Log("frame : " + frameCounter);
        float radius;
        for (int j = 0; j < _radiusPrecision; j++)
        {
            if (frameCounter == j) continue;
            radius = (MaxRadius - MinRadius) * ((float) j / _radiusPrecision) + MinRadius;
            for (int i = 0; i < _anglePrecision; i++)
            {
                Vector3 _direction = transform.up * GPCtrl.Instance.Player.Data.maxSwingDistance;

                switch (orientationMode)
                {
                    case OrientationMode.Y:
                        _direction += Quaternion.Euler(0, (float) i / _anglePrecision * 360, 0) * transform.forward * radius;
                        break;
                    case OrientationMode.X:
                        _direction += Quaternion.Euler((float) i / _anglePrecision * 360, 0, 0) * transform.forward * radius;
                        break;
                    case OrientationMode.Z:
                        _direction += Quaternion.Euler(0, 0, (float) i / _anglePrecision * 360) * transform.forward * radius;
                        break;
                }
                if (SearchPoint)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, _direction, out hit, GPCtrl.Instance.Player.Data.maxSwingDistance, layerMask))
                    {
                        contactPointList.Add(hit.point);
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        float radius;
        for (int j = 0; j < _radiusPrecision; j++)
        {
            radius = (MaxRadius - MinRadius) * ((float) j / _radiusPrecision) + MinRadius;
            for (int i = 0; i < _anglePrecision; i++)
            {
                Vector3 _direction = transform.up * 150;

                switch (orientationMode)
                {
                    case OrientationMode.Y:
                        _direction += Quaternion.Euler(0, (float) i / _anglePrecision * 360, 0) * transform.forward * radius;
                        break;
                    case OrientationMode.X:
                        _direction += Quaternion.Euler((float) i / _anglePrecision * 360, 0, 0) * transform.forward * radius;
                        break;
                    case OrientationMode.Z:
                        _direction += Quaternion.Euler(0, 0, (float) i / _anglePrecision * 360) * transform.forward * radius;
                        break;
                }
                switch (j)
                {
                    case 0:
                        Gizmos.color = Color.blue;
                        break;
                    case 1:
                        Gizmos.color = Color.yellow;
                        break;
                    case 2:
                        Gizmos.color = Color.green;
                        break;
                    case 3:
                        Gizmos.color = Color.red;
                        break;
                    default:
                        Gizmos.color = Color.black;
                        break;
                }
                //Debug.Log("draw line : " + _direction);
                Gizmos.DrawLine(transform.position, transform.position + (_direction));
            }
        }
    }
}