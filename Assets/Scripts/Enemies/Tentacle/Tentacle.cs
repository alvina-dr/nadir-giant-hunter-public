using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Tentacle : MonoBehaviour
{
    [TabGroup("Components")]
    public Transform StartTentacle;
    [TabGroup("Components")]
    public Transform EndTentacle;
    [TabGroup("Components")]
    public GameObject TentacleScalePrefab;

    [TabGroup("Parameters")]
    public bool IsByDensity;
    [TabGroup("Parameters"), ShowIf("IsByDensity")]
    public float TentacleDensity;
    [TabGroup("Parameters"), HideIf("IsByDensity")]
    public int TentacleNum;
    [TabGroup("Parameters")]
    public float StartTentaclePow;
    [TabGroup("Parameters")]
    public float EndTentaclePow;

    private BezierSpline bezierSpline;

    private string toDebug;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    [Button]
    public void GenerateTentacle()
    {
        float startToEndDist = Vector3.Distance(StartTentacle.position, EndTentacle.position);
        Vector3 p1 = StartTentacle.position + StartTentacle.forward * StartTentaclePow;
        Vector3 p2 = EndTentacle.position - EndTentacle.forward * EndTentaclePow;
        float length = BezierCurve.GetLength(StartTentacle.position, p1, p2, EndTentacle.position, (int)(startToEndDist * 10));
        int tentacleScaleNum = Mathf.RoundToInt(length * TentacleDensity);
        toDebug = "";
        toDebug += "startToEndDist : " + startToEndDist + "\n";
        toDebug += "tentacles scale num : " + tentacleScaleNum + "\n";
        Transform lastTentacle = StartTentacle;
        for (int i = 0; i < tentacleScaleNum; i++)
        {
            GameObject tentacleScale = Instantiate(TentacleScalePrefab, lastTentacle);
            lastTentacle = tentacleScale.transform;
            float delta = (float)i / tentacleScaleNum;
            tentacleScale.transform.position = BezierCurve.GetPoint(StartTentacle.position, p1, p2, EndTentacle.position, delta);
            float nextDelta = (float)(i+1) / tentacleScaleNum;
            Vector3 nextPointPos = BezierCurve.GetPoint(StartTentacle.position, StartTentacle.position + StartTentacle.forward * StartTentaclePow, EndTentacle.position - EndTentacle.forward * EndTentaclePow, EndTentacle.position, nextDelta);
            tentacleScale.transform.rotation = Quaternion.LookRotation(nextPointPos - tentacleScale.transform.position, Vector3.up);
        }

    }

    [Button]
    public void ResetTentacle()
    {
        DestroyImmediate(StartTentacle.GetChild(0).gameObject);
    }

    private void OnGUI()
    {
        GUIStyle gUIStyle = new GUIStyle();
        gUIStyle.alignment = TextAnchor.UpperLeft;
        GUILayout.Box("Tentacle Debug: \n" + toDebug, gUIStyle);
    }

    private void OnDrawGizmos()
    {
        float startToEndDist = Vector3.Distance(StartTentacle.position, EndTentacle.position);
        float tentacleScaleNum = startToEndDist * 6;
        Vector3[] points = new Vector3[(int)tentacleScaleNum+1];
        int count = 0;
        foreach (Vector3 point in points)
        {
            float delta = (float)count / (int)tentacleScaleNum;
            points[count] = BezierCurve.GetPoint(StartTentacle.position, StartTentacle.position + StartTentacle.forward * StartTentaclePow, EndTentacle.position - EndTentacle.forward * EndTentaclePow, EndTentacle.position, delta);
            count++;
        }
        Gizmos.DrawLineStrip(points, false);
    }
}
