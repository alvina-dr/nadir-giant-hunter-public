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
    public float CurrentRadius;
    [SerializeField] private int _anglePrecision;
    [SerializeField] int _radiusPrecision;

    private void Update()
    {
        contactPointList.Clear();
        float radius;
        for (int j = 0; j < _radiusPrecision; j++)
        {
            radius = (MaxRadius - MinRadius) * (j / (float) _radiusPrecision) + MinRadius;
            for (int i = 0; i < _anglePrecision; i++)
            {
                Vector3 _direction = transform.up * GPCtrl.Instance.Player.Data.maxSwingDistance;
                switch (orientationMode)
                {
                    case OrientationMode.Y:
                        _direction += Quaternion.Euler(0, i / _anglePrecision * 10, 0) * transform.forward * radius;
                        break;
                    case OrientationMode.X:
                        _direction += Quaternion.Euler(i / _anglePrecision * 10, 0, 0) * transform.forward * radius;
                        break;
                    case OrientationMode.Z:
                        _direction += Quaternion.Euler(0, 0, i / _anglePrecision * 10) * transform.forward * radius;
                        break;
                }
                Debug.DrawRay(transform.position, _direction, Color.red);
                if (!SearchPoint) return;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, _direction, out hit, GPCtrl.Instance.Player.Data.maxSwingDistance, layerMask))
                {
                    contactPointList.Add(hit.point);
                }
            }
        }
    }
}