using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DistanceComparer : IComparer<Vector3>
{
    private Vector3 point; // The point from which distances are measured

    public DistanceComparer(Vector3 point)
    {
        this.point = point;
    }

    public int Compare(Vector3 a, Vector3 b)
    {
        float distanceToA = Vector3.Distance(a, point);
        float distanceToB = Vector3.Distance(b, point);

        // Compare distances
        if (distanceToA < distanceToB)
            return -1;
        else if (distanceToA > distanceToB)
            return 1;
        else
            return 0;
    }
}


public class Climbing : MonoBehaviour
{
    struct Intersection
    {
        public Vector3 ObjPosition;
        public int[] Triangle1;
        public int[] Triangle2;
        public Intersection(Vector3 objPosition, int[] triangle1, int[] triangle2)
        {
            ObjPosition = objPosition;
            Triangle1 = triangle1;
            Triangle2 = triangle2;
        }
        public Intersection(Vector3 objPosition, int vert1, int vert2, int vert3)
        {
            ObjPosition = objPosition;
            Triangle1 = new int[3] { vert1, vert2, vert3 };
            Triangle2 = new int[3] { vert1, vert2, 0 };
        }
    }


    public float RayLength;
    public float DistanceToTravel;
    public int DebugSelected = 0;
    [SerializeField] private bool _automaticallyAttach = false;

    [Title("Debug")]
    [ReadOnly, SerializeField]
    private bool _isOnMesh;
    [ReadOnly, SerializeField]
    private GameObject _meshsObject;
    private Mesh _currentMesh;
    private SkinnedMeshRenderer _currentSkinnedMeshRenderer;
    private Mesh _bakedMesh;
    private int _triangleIndex;
    private Vector3 _barryCentricPos;
    private Vector3 _Normal;

    private Vector3 _currentObjPosition;

    private int[] vertIndex = new int[3];
    Vector3[] vertices = new Vector3[3];



    private List<Intersection> directionIntersections = new List<Intersection>();
    private List<Intersection> TravelPoints = new List<Intersection>();

    void Start()
    {
        _bakedMesh = new Mesh();
    }

    public bool IsOnMesh()
    {
        return _isOnMesh;
    }

    void Update()
    {
        if (_isOnMesh)
        {
            UpdatePosition();
        }
    }

    private void FixedUpdate()
    {
        if (!_isOnMesh && _automaticallyAttach)
        {
            RayCastForMesh();
        }
    }

    private void RayCastForMesh()
    {
        Ray ray = new Ray(transform.position, -transform.up * RayLength);
        Debug.DrawRay(transform.position, -transform.up * RayLength);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, RayLength))
        {
            if (!hit.collider.TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer SkinnedMesh))
            {
                return;
            }
            StartMeshClimbing(hit);
        }
    }

    [Button("Stop Mesh Climbing")]
    public void StopMeshClimbing()
    {
        _isOnMesh = false;
    }

    public void StartMeshClimbing(RaycastHit hit)
    {
        _currentSkinnedMeshRenderer = hit.collider.GetComponent<SkinnedMeshRenderer>();
        _meshsObject = hit.transform.parent.gameObject;
        _currentMesh = _currentSkinnedMeshRenderer.sharedMesh;
        _triangleIndex = hit.triangleIndex;
        _barryCentricPos = hit.barycentricCoordinate;
        _Normal = hit.normal;
        _isOnMesh = true;
        SnapOnMesh();
    }

    public void StartMeshClimbing(SkinnedMeshRenderer skinnedMesh, int triangleIndex, Vector3 barycentricCoordinate, Vector3 normal)
    {
        _currentSkinnedMeshRenderer = skinnedMesh;
        _meshsObject = skinnedMesh.transform.parent.gameObject;
        _currentMesh = _currentSkinnedMeshRenderer.sharedMesh;
        _triangleIndex = triangleIndex;
        _barryCentricPos = barycentricCoordinate;
        _Normal = normal;
        _isOnMesh = true;
        SnapOnMesh();
    }

    private void SnapOnMesh()
    {
        Mesh baked = new Mesh();
        _currentSkinnedMeshRenderer.BakeMesh(baked);

        vertIndex[0] = baked.triangles[_triangleIndex * 3];
        vertIndex[1] = baked.triangles[_triangleIndex * 3 + 1];
        vertIndex[2] = baked.triangles[_triangleIndex * 3 + 2];

        vertices[0] = baked.vertices[vertIndex[0]];
        vertices[1] = baked.vertices[vertIndex[1]];
        vertices[2] = baked.vertices[vertIndex[2]];


        _currentObjPosition = GetObjPositionFromBarycentric(_barryCentricPos, vertices[0], vertices[1], vertices[2]);
    }

    private void UpdatePosition()
    {
        _bakedMesh.Clear();
        _currentSkinnedMeshRenderer.BakeMesh(_bakedMesh);

        vertIndex[0] = _bakedMesh.triangles[_triangleIndex * 3];
        vertIndex[1] = _bakedMesh.triangles[_triangleIndex * 3 + 1];
        vertIndex[2] = _bakedMesh.triangles[_triangleIndex * 3 + 2];

        vertices[0] = _bakedMesh.vertices[vertIndex[0]];
        vertices[1] = _bakedMesh.vertices[vertIndex[1]];
        vertices[2] = _bakedMesh.vertices[vertIndex[2]];

        _currentObjPosition = GetObjPositionFromBarycentric(_barryCentricPos, vertices[0], vertices[1], vertices[2]);
        Vector3 position = _meshsObject.transform.position + _currentObjPosition;
        Quaternion rot = Quaternion.LookRotation(_bakedMesh.normals[vertIndex[0]]);
        transform.rotation = rot;
        transform.eulerAngles = new Vector3(rot.eulerAngles.x + 90, rot.eulerAngles.y, rot.eulerAngles.z);
        transform.position = position;
        
    }

    public void MoveToward(Vector3 plane, Vector3 direction, float distance) {
        GetIntersectingPoints(_bakedMesh.vertices, _currentObjPosition, plane);
        Intersection point = GetNextPosition(_currentObjPosition, direction, distance);
        if (point.ObjPosition == Vector3.zero)
        {
            return;
        }
        _triangleIndex = Mathf.FloorToInt(GetIndexFromInt(_bakedMesh.triangles, point.Triangle1[0])/3);
        _barryCentricPos = GetBaryCentricPosition(point.ObjPosition, _triangleIndex);
    }

    private int GetIndexFromInt(int[] array, int toFind)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == toFind)
            {
                return i;
            }
        }
        return -1;
    }

    private Vector3 GetBaryCentricPosition(Vector3 position, int triangleIndex)
    {
        vertIndex[0] = _bakedMesh.triangles[triangleIndex * 3];
        vertIndex[1] = _bakedMesh.triangles[triangleIndex * 3 + 1];
        vertIndex[2] = _bakedMesh.triangles[triangleIndex * 3 + 2];

        vertices[0] = _bakedMesh.vertices[vertIndex[0]];
        vertices[1] = _bakedMesh.vertices[vertIndex[1]];
        vertices[2] = _bakedMesh.vertices[vertIndex[2]];


        Vector3 v0 = vertices[1] - vertices[0], v1 = vertices[2] - vertices[0], v2 = position - vertices[0];
        float d00 = Vector3.Dot(v0, v0);
        float d01 = Vector3.Dot(v0, v1);
        float d11 = Vector3.Dot(v1, v1);
        float d20 = Vector3.Dot(v2, v0);
        float d21 = Vector3.Dot(v2, v1);
        float denom = d00 * d11 - d01 * d01;

        float v = (d11 * d20 - d01 * d21) / denom;
        float w = (d00 * d21 - d01 * d20) / denom;
        float u = 1.0f - v - w;
        return new Vector3(u,v,w);
    }

    //Returns a position in object space from barycentric coord and its vertexes
    private Vector3 GetObjPositionFromBarycentric(Vector3 barycentricCoords, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        float u = barycentricCoords.x;
        float v = barycentricCoords.y;
        float w = barycentricCoords.z;

        Vector3 worldPosition = u * vertex1 + v * vertex2 + w * vertex3;
        return worldPosition;
    }

    void GetIntersectingPoints(Vector3[] vertices, Vector3 BarryCentricPosition, Vector3 plane)
    {
        Vector3[] edges = new Vector3[vertices.Length];
        directionIntersections = new List<Intersection>();
        for (int i = 0; i < vertices.Length/3; i++)
        {
            int index = i * 3;
            edges[index] = vertices[index + 1] - vertices[index];
            edges[index + 1] = vertices[index + 2] - vertices[index + 1];
            edges[index + 2] = vertices[index] - vertices[index + 2];

            Vector3 point = IntersectPoint(edges[index], vertices[index], plane, BarryCentricPosition);
            if (point != Vector3.one * 10000)
            {
                directionIntersections.Add(new Intersection(point, index, index+1, index+2));

            }
            point = IntersectPoint(edges[index + 1], vertices[index + 1], plane, BarryCentricPosition);
            if (point != Vector3.one * 10000)
            {
                directionIntersections.Add(new Intersection(point, index, index+1, index+2));

            }
            point = IntersectPoint(edges[index + 2], vertices[index + 2], plane, BarryCentricPosition);
            if (point != Vector3.one * 10000)
            {
                directionIntersections.Add(new Intersection(point, index, index+1, index+2));

            }
        }

    }

    Vector3 IntersectPoint(Vector3 rayVector, Vector3 rayPosition, Vector3 planeNormal, Vector3 planePosition)
    {
        Vector3 diff = rayPosition - planePosition;
        float prod1 = Vector3.Dot(diff, planeNormal);
        float prod2 = Vector3.Dot(rayVector, planeNormal);
        float prod3 = prod1 / prod2;
        Vector3 point = rayPosition - rayVector * prod3;
        if (Mathf.Abs((point - (rayPosition + rayVector/2)).magnitude) > rayVector.magnitude / 2)
        {
            return Vector3.one * 10000;
        }
        return point;
    }

    //Get the next position of the player after movement (position in object space)
    Intersection GetNextPosition(Vector3 BarryCentricPosition, Vector3 direction, float distance)
    {
        float currentDist = 0;
        Vector3 lastPoint = BarryCentricPosition;
        TravelPoints.Clear();
        for (int i = 1; i < directionIntersections.Count; i++)
        {
            int nextPointIndex = GetNextIntersection(directionIntersections, lastPoint, BarryCentricPosition, direction, 0);
            if(nextPointIndex == -1)
            {
                break;
            }
            if (currentDist + (directionIntersections[nextPointIndex].ObjPosition - lastPoint).magnitude > distance)
            {
                Vector3 position = lastPoint + (directionIntersections[nextPointIndex].ObjPosition - lastPoint).normalized * (distance - currentDist);
                vertIndex[0] = _bakedMesh.triangles[_triangleIndex * 3];
                vertIndex[1] = _bakedMesh.triangles[_triangleIndex * 3 + 1];
                vertIndex[2] = _bakedMesh.triangles[_triangleIndex * 3 + 2];

                Intersection last = TravelPoints.Count == 0 ? new Intersection(Vector3.zero, vertIndex[0], vertIndex[1], vertIndex[2]) : TravelPoints[TravelPoints.Count - 1];
                TravelPoints.Add(new Intersection(position, last.Triangle1[0], last.Triangle1[1], last.Triangle1[2]));
                break;
            }
            currentDist += (directionIntersections[nextPointIndex].ObjPosition - lastPoint).magnitude;
            lastPoint = directionIntersections[nextPointIndex].ObjPosition;
            TravelPoints.Add(directionIntersections[nextPointIndex]);
            directionIntersections.RemoveAt(nextPointIndex);
        }
        return TravelPoints.Count == 0 ? new Intersection(Vector3.zero, 0,0,0) : TravelPoints[TravelPoints.Count - 1];
    }

    //Returns the index of the next intersection forward direction
    int GetNextIntersection(List<Intersection> intersections, Vector3 position, Vector3 firstPos, Vector3 direction, float minDot)
    {
        int index = -1;
        for (int i = 0; i < intersections.Count; i++)
        {
            float currentDist = (intersections[i].ObjPosition - position).magnitude;
            float closestDist = index == -1 ? 1000000 : (intersections[index].ObjPosition - position).magnitude;
            if (Vector3.Dot(intersections[i].ObjPosition - firstPos, direction) < minDot || currentDist >= closestDist || intersections[i].ObjPosition == position)
            {
                continue;
            }
            index = i;
        }
        return index;
    }


    private void OnDrawGizmos()
    {
        if (TravelPoints == null)
        {
            return;
        }
        for (int i = 0; i < TravelPoints.Count; i++)
        {
            if (TravelPoints[i].ObjPosition == Vector3.one * 10000)
            {
                continue;
            }
            Gizmos.color = Color.red;
            if (DebugSelected == i)
            {
                Gizmos.color = Color.green;
            }
            Gizmos.DrawSphere(_meshsObject.transform.position + TravelPoints[i].ObjPosition, 0.1f);
        }
    }
}
