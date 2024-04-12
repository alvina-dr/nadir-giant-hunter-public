using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    public EnemyData enemyData;
    [TitleGroup("Components")]
    public Transform toGo;

    [TitleGroup("Parameters")]
    public LayerMask GroundLayer;
    [SerializeField]
    private Vector3 _direction;
    [TitleGroup("Debug")]
    [SerializeField, ReadOnly]
    private Vector3 _positionWanted;



    // Start is called before the first frame update
    void Start()
    {
        _positionWanted = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }



    private void Move()
    {
        _positionWanted += _direction * enemyData.MoveSpeed * Time.fixedDeltaTime;
        if (toGo!=null)
        {
            _positionWanted = toGo.position;
        }
        transform.position += (_positionWanted - transform.position).normalized * enemyData.MoveSpeed * Time.fixedDeltaTime;
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
        Vector3 posWanted = hitInfo.point - dir.normalized * enemyData.DistanceToGround;
        transform.position = Vector3.Slerp(transform.position, posWanted, 0.9f * Time.fixedDeltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_positionWanted, 2);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward*1000);
    }
}
