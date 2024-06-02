using Enemies;
using Mono.Cecil.Cil;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

namespace Enemies
{
    [Serializable]
    public class TentacleScaleAnim
    {
        public AnimationCurve Curve;
        public TentacleScale.DeltaType DeltaType;
        public Vector2 CursorRange;
        [PropertyRange(-1, 2)]
        public float CursorPosition;
        public float AnimationTime;
        public float AnimationScale;
        public bool isAnimationReverse;
        public bool DoAnimationLoop;
        public bool DoAnimationOnStart;
        [ReadOnly]
        public bool IsAnimating;
    }

    public class Tentacle : MonoBehaviour
    {
        [TabGroup("Components")]
        public Transform StartTentaclePos;
        [TabGroup("Components")]
        public Transform EndTentaclePos;
        [TabGroup("Components")]
        public GameObject TentacleScalePrefab;
        [TabGroup("Components")]
        public Transform IkTarget;
        [TabGroup("Components")]
        public RigBuilder TentRigBuilder;
        [TabGroup("Components")]
        public Animator EnemyAnimator;
        public GameObject StartTentacleScale;
        public GameObject EndTentacleScale;
        private ChainIKConstraint TentacleIKConstraint;
        private TentacleScale [] tentacleScales;
        private Rigidbody[] rigidbodies;

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

        [TabGroup("ScaleAnim")]
        public List<TentacleScaleAnim> TentacleScaleAnimations = new List<TentacleScaleAnim>();


        private string toDebug;


        // Start is called before the first frame update
        void Start()
        {
            TentacleIKConstraint = GetComponent<ChainIKConstraint>();
            rigidbodies = GetComponentsInChildren<Rigidbody>();
            RetrieveAllTentacleScales();
            StartAnimationsOnStart();
        }

        // Update is called once per frame
        void Update()
        {
            SendTentacleScaleAnimData();
        }

        #region TentacleGeneration
        [Button]
        public void GenerateTentacle()
        {
            float startToEndDist = Vector3.Distance(StartTentaclePos.position, EndTentaclePos.position);
            Vector3 p1 = StartTentaclePos.position + StartTentaclePos.forward * StartTentaclePow;
            Vector3 p2 = EndTentaclePos.position - EndTentaclePos.forward * EndTentaclePow;
            Vector3[] points = new Vector3[] { StartTentaclePos.position, p1, p2 , EndTentaclePos.position };
            Bezier bezier = new Bezier(points, 1000);
            float length = bezier.TotalLength;
            int tentacleScaleNum = Mathf.RoundToInt(length * TentacleDensity);
            toDebug = "";
            toDebug += "startToEndDist : " + startToEndDist + "\n";
            toDebug += "tentacles scale num : " + tentacleScaleNum + "\n";
            Transform lastTentacle = StartTentaclePos;
            for (int i = 0; i < tentacleScaleNum; i++)
            {
                GameObject tentacleScale = Instantiate(TentacleScalePrefab, lastTentacle);
                lastTentacle = tentacleScale.transform;
                float delta = (float)i / tentacleScaleNum;
                tentacleScale.transform.position = bezier.GetPoint(delta);
                float nextDelta = (float)(i+1) / tentacleScaleNum;
                Vector3 nextPointPos = bezier.GetPoint(nextDelta);
                tentacleScale.transform.rotation = Quaternion.LookRotation(nextPointPos - tentacleScale.transform.position, Vector3.up);
                if (i == tentacleScaleNum - 1)
                    EndTentacleScale = tentacleScale;
                if (i == 0)
                    StartTentacleScale = tentacleScale;
            }
            RetrieveAllTentacleScales();
        }

        [Button]
        public void ResetTentacle()
        {
            EndTentacleScale = null;
            StartTentacleScale = null;
            DestroyImmediate(StartTentaclePos.GetChild(0).gameObject);
        }

        public void RetrieveAllTentacleScales()
        {
            tentacleScales = GetComponentsInChildren<TentacleScale>();
        }

        #endregion

        #region IK
        [Button]
        public void SetupIKConstraint()
        {
            if (!TentacleIKConstraint)
            {
                TentacleIKConstraint = GetComponent<ChainIKConstraint>();
            }
            TentacleIKConstraint.data.root = StartTentacleScale.transform;
            TentacleIKConstraint.data.tip = EndTentacleScale.transform;
            TentacleIKConstraint.data.target = IkTarget;
            if (TentRigBuilder && Application.isPlaying)
            {
                TentRigBuilder.Build();
            }
        }

        #endregion

        #region RagDoll

        [Button]
        public void SetupRagDoll()
        {
            SetupRagdollPart(EndTentacleScale);
        }

        private void SetupRagdollPart(GameObject part)
        {
            if (part.transform.parent == StartTentaclePos)
            {
                return;
            }
            CharacterJoint characterJoint = part.GetComponent<CharacterJoint>();
            characterJoint.connectedBody = part.transform.parent.GetComponent<Rigidbody>();
            characterJoint.autoConfigureConnectedAnchor = true;
            SetupRagdollPart(part.transform.parent.gameObject);
        }

        public void SetRagdoll(bool isActive)
        {
            //Deactivate everything that will impair ragdoll
            EnemyAnimator.enabled = !isActive;

            foreach (var rigidbody in rigidbodies)
            {
                rigidbody.isKinematic = !isActive;
                if (isActive)
                {
                    rigidbody.velocity = Vector3.zero;
                }
            }
        }
        [Button]
        public void ActivateRagdoll()
        {
            SetRagdoll(true);
        }

        [Button]
        public void DeactivateRagdoll()
        {
            SetRagdoll(false);
        }


        #endregion


        #region TentacleScaleAnimation

        private void SendTentacleScaleAnimData()
        {
            for (int i = 0; i < tentacleScales.Length; i++)
            {
                TentacleScale tentacleScale = tentacleScales[i];
                foreach (TentacleScaleAnim anim in TentacleScaleAnimations)
                {
                    float tentacleDeltaAdd = (i/(float)tentacleScales.Length);
                    float delta = anim.Curve.Evaluate(anim.CursorPosition+ tentacleDeltaAdd*anim.AnimationScale);
                    tentacleScale.AddDelta(delta, anim.DeltaType);
                }
            }
        }

        public void StartAnimationsOnStart()
        {
            foreach (TentacleScaleAnim anim in TentacleScaleAnimations)
            {
                if (!anim.DoAnimationOnStart)
                {
                    continue;
                }
                StartCoroutine(TentacleAnimation(anim));
            }
        }

        [Button]
        public void StartTentacleAnimationByIndex(int index)
        {
            if (TentacleScaleAnimations[index].IsAnimating)
                return;
            StartCoroutine(TentacleAnimation(TentacleScaleAnimations[index]));
        }

        private IEnumerator TentacleAnimation(TentacleScaleAnim anim)
        {
            float startTime = Time.time;
            float endTime = Time.time + anim.AnimationTime;

        
            float delta = 0;
            while (Time.time < endTime || anim.DoAnimationLoop)
            {
                delta += Time.deltaTime / anim.AnimationTime;
                float cursorPos = Mathf.Lerp(anim.CursorRange.x, anim.CursorRange.y, anim.isAnimationReverse ? delta : 1 - delta);
                anim.CursorPosition = cursorPos;
                yield return 0;
                if (anim.DoAnimationLoop && Time.time >= endTime)
                {
                    delta = 0;
                    startTime = Time.time;
                    endTime = Time.time + anim.AnimationTime;
                }
            }
        }

        #endregion


        private void OnGUI()
        {
            GUIStyle gUIStyle = new GUIStyle();
            gUIStyle.alignment = TextAnchor.UpperLeft;
            GUILayout.Box("Tentacle Debug: \n" + toDebug, gUIStyle);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            float startToEndDist = Vector3.Distance(StartTentaclePos.position, EndTentaclePos.position);
            float tentacleScaleNum = startToEndDist * 6;
            Vector3[] points = new Vector3[(int)tentacleScaleNum+1];
            int count = 0;
            foreach (Vector3 point in points)
            {
                float delta = (float)count / (int)tentacleScaleNum;
                points[count] = Bezier.GetPoint(StartTentaclePos.position, StartTentaclePos.position + StartTentaclePos.forward * StartTentaclePow, EndTentaclePos.position - EndTentaclePos.forward * EndTentaclePow, EndTentaclePos.position, delta);
                count++;
            }
            Gizmos.DrawLineStrip(points, false);
        }
    }
}

