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
    public bool searchPoint = false;
    public float minRadius;
    public float maxRadius;
    public float radius;
    public float precision;

    private void Update()
    {
        contactPointList.Clear();
        for (int i = 0; i < 360 * precision; i++)
        {
            Vector3 _direction = transform.up * GPCtrl.Instance.Player.Data.maxSwingDistance;
            switch (orientationMode)
            {
                case OrientationMode.Y:
                    _direction += Quaternion.Euler(0, i / precision * 10, 0) * transform.forward * radius;
                    break;
                case OrientationMode.X:
                    _direction += Quaternion.Euler(i / precision * 10, 0, 0) * transform.forward * radius;
                    break;
                case OrientationMode.Z:
                    _direction += Quaternion.Euler(0, 0, i / precision * 10) * transform.forward * radius;
                    break;
            }
            Debug.DrawRay(transform.position, _direction, Color.red);
            if (!searchPoint) return;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, _direction, out hit, GPCtrl.Instance.Player.Data.maxSwingDistance, layerMask))
            {
                contactPointList.Add(hit.point);
            }
        }
    }
}
