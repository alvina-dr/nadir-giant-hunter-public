using DG.Tweening;
using Enemies;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Profiling;
using UnityEngine.UIElements;
using static ak.wwise.core;

namespace Enemies
{
    [Serializable]
    public class TentacleScaleAnim
    {
        public AnimationCurve Curve;
        public TentacleScale.DeltaType DeltaType;
        public Vector2 CursorRange;
        [PropertyRange(-3, 3)]
        public float CursorPosition;
        public float AnimationTime;
        public float AnimationScale;
        public bool isAnimationReverse;
        public bool DoAnimationLoop;
        public bool DoAnimationOnStart;
        [Sirenix.OdinInspector.ReadOnly]
        public bool IsAnimating;
    }

    public class Tentacle : MonoBehaviour
    {
        public enum dir
        {
            forward,
            right,
            up,
            backward,
            left,
            down
        }

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
        [TabGroup("Components")]
        public Leg leg;
        public GameObject StartTentacleScale;
        public GameObject EndTentacleScale;
        private ChainIKConstraint TentacleIKConstraint;
        [HideInInspector]
        public TentacleScale[] tentacleScales;
        [HideInInspector]
        public Transform[] bones;
        private Rigidbody[] rigidbodies;

        [TabGroup("Parameters")]
        public bool isNotTentacleScales;
        [TabGroup("Parameters")]
        public bool IsByDensity;
        [TabGroup("Parameters"), ShowIf("IsByDensity")]
        public float TentacleDensity;
        [TabGroup("Parameters"), HideIf("IsByDensity")]
        public int TentacleNum;
        [TabGroup("Parameters")]
        public float RotationOffset;
        private float _currentRotationOffset;
        [TabGroup("Parameters")]
        public float StartTentaclePow;
        [TabGroup("Parameters")]
        public dir StartTentacleDir;
        [TabGroup("Parameters")]
        public float EndTentaclePow;
        [TabGroup("Parameters"), PropertyRange(0f, 1f)]
        public float deltaGrounded;

        [TabGroup("ScaleAnim")]
        public List<TentacleScaleAnim> TentacleScaleAnimations = new List<TentacleScaleAnim>();


        private string toDebug;


        void Start()
        {
            TentacleIKConstraint = GetComponent<ChainIKConstraint>();
            rigidbodies = GetComponentsInChildren<Rigidbody>();
            RetrieveAllTentacleScales();
            StartAnimationsOnStart();
        }

        void Update()
        {
            deltaGrounded = leg.GroundedDelta;
            SendTentacleScaleAnimData();
        }

        private void LateUpdate()
        {

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
            _currentRotationOffset = 0;
            for (int i = 0; i < tentacleScaleNum; i++)
            {
                GameObject tentacleScale = Instantiate(TentacleScalePrefab, lastTentacle);
                lastTentacle = tentacleScale.transform;
                float delta = (float)i / tentacleScaleNum;
                tentacleScale.transform.position = bezier.GetPoint(delta);
                float nextDelta = (float)(i+1) / tentacleScaleNum;
                Vector3 nextPointPos = bezier.GetPoint(nextDelta);
                tentacleScale.transform.rotation = Quaternion.LookRotation(nextPointPos - tentacleScale.transform.position, Vector3.up);
                //tentacleScale.transform.GetChild(0).localRotation *= Quaternion.AngleAxis(_currentRotationOffset+RotationOffset, Vector3.forward);
                _currentRotationOffset += RotationOffset;
                if (i == tentacleScaleNum - 1)
                    EndTentacleScale = tentacleScale;
                if (i == 0)
                    StartTentacleScale = tentacleScale;
                TentacleScale tentacle = tentacleScale.GetComponent<TentacleScale>();
                tentacle.CurrentFrame = i % tentacle.FrameEco;
            }
            RetrieveAllTentacleScales();
        }

        [Button]
        public void AlignTentacle()
        {
            float startToEndDist = Vector3.Distance(StartTentaclePos.position, EndTentaclePos.position);
            Vector3 startDir = Vector3.zero;
            switch (StartTentacleDir)
            {
                case dir.forward:
                    startDir = StartTentaclePos.forward;
                    break;
                case dir.right:
                    startDir = StartTentaclePos.right;
                    break;
                case dir.up:
                    startDir = StartTentaclePos.up;
                    break;
                case dir.backward:
                    startDir = -StartTentaclePos.forward;
                    break;
                case dir.left:
                    startDir = -StartTentaclePos.right;
                    break;
                case dir.down:
                    startDir = -StartTentaclePos.up;
                    break;
            }
            Vector3 p1 = StartTentaclePos.position + startDir * StartTentaclePow;
            Vector3 p2 = EndTentaclePos.position - EndTentaclePos.forward * EndTentaclePow;
            Vector3[] points = new Vector3[] { StartTentaclePos.position, p1, p2, EndTentaclePos.position };
            Bezier bezier = new Bezier(points, 1000);
            float length = bezier.TotalLength;
            Transform[] tentaclesBones = StartTentaclePos.GetComponentsInChildren<Transform>();
            int tentacleScaleNum = tentaclesBones.Length;
            Transform lastTentacle = StartTentaclePos;
            for (int i = 0; i < tentacleScaleNum; i++)
            {
                Transform tentacleBone = tentaclesBones[i];
                //Position
                float delta = (float)i / tentacleScaleNum;
                tentacleBone.position = bezier.GetPoint(delta);

                //Rotation
                float nextDelta = (float)(i + 1) / tentacleScaleNum;
                Vector3 nextPointPos = bezier.GetPoint(nextDelta);
                tentacleBone.rotation = Quaternion.LookRotation(nextPointPos - tentacleBone.position, Vector3.up);
                tentacleBone.rotation *= Quaternion.AngleAxis(90, Vector3.right);

                if (i == tentacleScaleNum - 1)
                    EndTentacleScale = tentacleBone.gameObject;
                if (i == 0)
                    StartTentacleScale = tentacleBone.gameObject;
            }
            RetrieveAllBones();
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

        public void RetrieveAllBones()
        {
            bones = StartTentaclePos.GetComponentsInChildren<Transform>();
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
            rigidbodies = GetComponentsInChildren<Rigidbody>();

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
            //GUIStyle gUIStyle = new GUIStyle();
            //gUIStyle.alignment = TextAnchor.UpperLeft;
            //GUILayout.Box("Tentacle Debug: \n" + toDebug, gUIStyle);
        }

        private void OnDrawGizmos()
        {
            /*Gizmos.color = Color.blue;
            float startToEndDist = Vector3.Distance(StartTentaclePos.position, EndTentaclePos.position);
            float tentacleScaleNum = startToEndDist * 6;
            Vector3[] points = new Vector3[(int)tentacleScaleNum+1];
            int count = 0;
            foreach (Vector3 point in points)
            {
                float delta = (float)count / (int)tentacleScaleNum;
                Vector3 startDir = Vector3.zero;
                switch (StartTentacleDir)
                {
                    case dir.forward:
                        startDir = StartTentaclePos.forward;
                        break;
                    case dir.right:
                        startDir = StartTentaclePos.right;
                        break;
                    case dir.up:
                        startDir = StartTentaclePos.up;
                        break;
                    case dir.backward:
                        startDir = -StartTentaclePos.forward;
                        break;
                    case dir.left:
                        startDir = -StartTentaclePos.right;
                        break;
                    case dir.down:
                        startDir = -StartTentaclePos.up;
                        break;
                }
                Vector3 p1 = StartTentaclePos.position + startDir * StartTentaclePow;
                points[count] = Bezier.GetPoint(StartTentaclePos.position, p1, EndTentaclePos.position - EndTentaclePos.forward * EndTentaclePow, EndTentaclePos.position, delta);
                count++;
            }
            Gizmos.DrawLineStrip(points, false);*/
        }
    }
}