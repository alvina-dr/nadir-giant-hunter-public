using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [TitleGroup("Components")]
    public EnemyData Data;
    [TitleGroup("Components")]
    public Transform toGo;

    [TitleGroup("Parameters")]
    public LayerMask GroundLayer;
    [SerializeField]
    private Vector3 _direction;
    [TitleGroup("Debug")]
    [SerializeField, ReadOnly]
    private Vector3 _positionWanted;

    void Start()
    {
        _positionWanted = transform.position;
    }

    private void Update()
    {
        if (GPCtrl.Instance.Pause) return;
        if (transform.position.y > GPCtrl.Instance.GeneralData.yHeightGameOver)
        {
            GPCtrl.Instance.Loose(this);
        }
    }

    void FixedUpdate()
    {
        if (GPCtrl.Instance.Pause) return;
        Move();
    }

    private void Move()
    {
        float moveSpeed = Mathf.Lerp(Data.InitialSpeed, Data.FinalSpeed, Mathf.Min(transform.position.y / Data.FinalSpeedHeight, 1));
        _positionWanted += _direction * moveSpeed * Time.fixedDeltaTime;
        if (toGo!=null)
        {
            _positionWanted = toGo.position;
        }
        transform.position += (_positionWanted - transform.position).normalized * moveSpeed * Time.fixedDeltaTime;
        GetDown();
    }

    private void GetDown()
    {
        Ray ray = new Ray(transform.position, transform.forward*1000);

        if (!Physics.Raycast(ray, out RaycastHit hitInfo, 1000, GroundLayer))
        {
            return;
        }
        Vector3 dir = hitInfo.point - transform.position;
        Vector3 posWanted = hitInfo.point - dir.normalized * Data.DistanceToGround;
        transform.position = Vector3.Slerp(transform.position, posWanted, 0.9f * Time.fixedDeltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_positionWanted, 2);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward*1000);
    }
}
