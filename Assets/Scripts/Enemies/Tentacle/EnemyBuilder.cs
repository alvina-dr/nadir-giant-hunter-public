using Enemies;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class EnemyBuilder : MonoBehaviour
    {
        [TabGroup("Components")]
        public GameObject TentaclePrefab;
        [TabGroup("Components")]
        public Transform TentacleParent;
        private IKHarmAnimation _iKHarmAnimation;

        [TabGroup("Parameters"), Tooltip("Need to be even")]
        public int LegNumber;
        [TabGroup("Parameters")]
        public Vector2 LegSpacing;
        [TabGroup("Parameters")]
        public float LegAngleSpacing;



        // Start is called before the first frame update
        void Start()
        {
            _iKHarmAnimation = GetComponent<IKHarmAnimation>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        [Button]
        public void BuildEnemy()
        {
            if (!_iKHarmAnimation)
                _iKHarmAnimation = GetComponent<IKHarmAnimation>();
            LegNumber = LegNumber % 2 > 1 ? LegNumber - 1 : LegNumber;
            Transform[] ToOrganize = new Transform[LegNumber];
            int startInd = LegNumber / 2 + ((LegNumber/2) % 2 >= 1 ? 1:2);
            for (int i = 0; i < LegNumber; i++)
            {
                int index = i;
                float angle = 90;
                int orgIndex = startInd + 2 * (index % 2 < 1 ? (int)Mathf.Ceil((float)index / 2) : -(int)Mathf.Ceil((float)index / 2));
                Vector3 pos = transform.position;
                pos += transform.right * LegSpacing.x;
                if (i >= LegNumber / 2)
                {
                    angle = -90;
                    index = i - LegNumber / 2;
                    pos -= transform.right * LegSpacing.x;
                    if ((LegNumber / 2) % 2 < 1)
                    {
                        startInd = LegNumber / 2 + ((LegNumber / 2) % 2 >= 1 ? 1 : 2) - 3;
                    }
                    else
                    {
                        startInd = LegNumber / 2 + ((LegNumber / 2) % 2 >= 1 ? 1 : 2) - 1;
                    }
                    orgIndex = startInd + 2 * (index % 2 < 1 ? -(int)Mathf.Ceil((float)index / 2) : (int)Mathf.Ceil((float)index / 2));
                }
                if ((LegNumber / 2) % 2 >= 1)
                {
                    angle -= LegAngleSpacing / 2;
                    pos += transform.forward * LegSpacing.y * (i >= LegNumber / 2 ? -1 : 1);
                }

                angle += index % 2 < 1 ? (index + 1) * LegAngleSpacing / 2 : (index) * -LegAngleSpacing / 2;
                pos += -transform.forward * (index % 2 < 1 ? (index + 1) * LegSpacing.y : (index) * -LegSpacing.y) * (i >= LegNumber / 2 ? -1 : 1);
            
                GameObject tentacleObj = Instantiate(TentaclePrefab, TentacleParent);
                ToOrganize[orgIndex-1] = tentacleObj.transform;
                tentacleObj.name = TentaclePrefab.name + "-" + i;
                Tentacle tentacle = tentacleObj.GetComponent<Tentacle>();
                tentacle.transform.position = pos;
                tentacle.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                tentacle.GenerateTentacle();
                tentacle.SetupIKConstraint();
                tentacle.SetupRagDoll();
            }
            for (int i = 0; i < ToOrganize.Length; i++)
            {
                ToOrganize[i].SetSiblingIndex(i);
            }
            _iKHarmAnimation.SetupLegs(TentacleParent);
        }

        [Button]
        public void ResetEnemy()
        {
            if (!_iKHarmAnimation)
                _iKHarmAnimation = GetComponent<IKHarmAnimation>();
            _iKHarmAnimation.ResetLegs();
            int count = TentacleParent.childCount;
            for (int i = 0; i < count; i++)
            {
                DestroyImmediate(TentacleParent.GetChild(0).gameObject);
            }
        }



        private void OnDrawGizmosSelected()
        {
            Tentacle tentacle = TentaclePrefab.GetComponent<Tentacle>();

            for (int i = 0; i < LegNumber; i++)
            {
                int index = i;
                float angle = 90;
                Vector3 pos = transform.position;
                pos += transform.right * LegSpacing.x;
                if (i >= LegNumber / 2)
                {
                    angle = -90;
                    index = i - LegNumber/2;
                    pos -= transform.right * LegSpacing.x*2;
                }
                if ((LegNumber / 2) % 2 >= 1)
                {
                    angle -= LegAngleSpacing/2;
                    pos += transform.forward * LegSpacing.y * (i >= LegNumber / 2 ? -1 : 1);
                }

                angle += index % 2 < 1 ? (index + 1)*LegAngleSpacing/2 : (index) *-LegAngleSpacing/2;
                pos += -transform.forward * (index % 2 < 1 ? (index + 1) * LegSpacing.y : (index) * -LegSpacing.y) * (i >= LegNumber / 2 ? -1 : 1);
            
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
}
