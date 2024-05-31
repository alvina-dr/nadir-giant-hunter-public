using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBuilder : MonoBehaviour
{
    [TabGroup("Components")]
    public GameObject TentaclePrefab;

    [TabGroup("Parameters"), Tooltip("Need to be even")]
    public int LegNumber;
    [TabGroup("Parameters")]
    public float LegSpacing;
    [TabGroup("Parameters")]
    public float LegAngleSpacing;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Button]
    public void BuildEnemy()
    {

        /*LegNumber = 1 % 2 > 1 ? Mathf.FloorToInt(LegNumber) : LegNumber;
        for (int i = 0; i < LegNumber; i++)
        {
            float angle = 90;
            if (i >= LegNumber / 2)
            {
                angle = 270;
            }

            angle += i % 2 < 1 ? (i + 1) * LegAngleSpacing : (i) * -LegAngleSpacing;
            Vector3 pos = transform.position + transform.forward * (i % 2 < 1 ? (i + 1) * LegSpacing : (i) * -LegSpacing);
            if ((LegNumber / 2) % 2 < 1)
            {
                angle -= LegAngleSpacing / 2;
                pos -= transform.forward * (LegSpacing / 2);
            }
            Tentacle tentacle = Instantiate(TentaclePrefab, );
        }*/
    }



    private void OnDrawGizmosSelected()
    {
        Tentacle tentacle = TentaclePrefab.GetComponent<Tentacle>();

        LegNumber = 1 % 2 > 1 ? Mathf.FloorToInt(LegNumber) : LegNumber;
        for (int i = 0; i < LegNumber/2; i++)
        {
            float angle = 90;
            if (i >= LegNumber / 2)
            {
                angle = 270;
            }
            
            angle += i % 2 < 1 ? (i+1)*LegAngleSpacing : (i)*-LegAngleSpacing;
            Vector3 pos = transform.position + -transform.forward * (i % 2 < 1 ? (i + 1) * LegSpacing : (i) * -LegSpacing);
            if ((LegNumber / 2) % 2 >= 1)
            {
                angle -= LegAngleSpacing / 2 * (i % 2 < 1 ? -1 : 1);
                pos += transform.forward * (LegSpacing / 2);
            }
            GetFourBezierPoint(pos, tentacle, angle, out Vector3 p0, out Vector3 p1, out Vector3 p2, out Vector3 p3);

            Gizmos.color = Color.blue;

            float startToEndDist = Vector3.Distance(tentacle.StartTentaclePos.position, tentacle.EndTentaclePos.position);
            float tentacleScaleNum = startToEndDist * 6;
            Vector3[] points = new Vector3[(int)tentacleScaleNum + 1];
            int count = 0;
            foreach (Vector3 point in points)
            {
                float delta = (float)count / (int)tentacleScaleNum;
                points[count] = Bezier.GetPoint(p0, p1, p2, p3, delta);
                count++;
            }
            Gizmos.DrawLineStrip(points, false);
        }

    }

    private void GetFourBezierPoint(Vector3 basePos, Tentacle tentacle, float angle, out Vector3 p0, out Vector3 p1, out Vector3 p2, out Vector3 p3)
    {
        p0 = tentacle.StartTentaclePos.localPosition;
        p3 = tentacle.EndTentaclePos.localPosition;
        p1 = p0 + tentacle.StartTentaclePos.forward * tentacle.StartTentaclePow;
        p2 = p3 - tentacle.EndTentaclePos.forward * tentacle.EndTentaclePow;
        p0 = basePos + Quaternion.AngleAxis(angle, Vector3.up) * p0;
        p3 = basePos + Quaternion.AngleAxis(angle, Vector3.up) * p3;
        p1 = basePos + Quaternion.AngleAxis(angle, Vector3.up) * p1;
        p2 = basePos + Quaternion.AngleAxis(angle, Vector3.up) * p2;
    }
}
