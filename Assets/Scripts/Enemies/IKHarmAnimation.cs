using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Sirenix.OdinInspector;
using DG.Tweening;
using System;
using static ak.wwise.core;
using UnityEngine.UIElements;
using UnityEditor;
using NUnit.Framework.Internal;

namespace Enemies
{
    [Serializable]
    public class Leg
    {
        public bool UseTwoBonesIk = true;
        [ShowIf("UseTwoBonesIk")]
        public TwoBoneIKConstraint Ik;
        [HideIf("UseTwoBonesIk")]
        public ChainIKConstraint ChainIk;
        public Transform Target;
        public Transform TargetPos;
        public GameObject LastBone;
        public GameObject FirstBone;
        [HideInInspector]
        public Tentacle tentacle;
        [HideInInspector]
        public Vector3 LastPosTarg;
        [HideInInspector]
        public float LastPosTargTotDist;
        [HideInInspector]
        public float MoveTime;
        [HideInInspector]
        public Vector3 LastPos;
        [HideInInspector]
        public float MaxLength;
        [ReadOnly]
        public bool DidLegJustTouchedGround = false;
        public float GroundedDelta = 1;
        public float LegSpeed;
        public List<int> ScalesAnimIndexOnGroundTouched = new List<int>();
        [HideInInspector]
        public float randomizer;
        //PERFORMANCE
        public float CurrentFrame = 0;
    }
    [Serializable]
    public class IkLegPair {
        public int CurrentLeg;
        public Leg[] Legs = new Leg[2];
    }


    public class IKHarmAnimation : MonoBehaviour
    {
        //Components
        [TitleGroup("Components")]
        public List<IkLegPair> _iksLegPairs = new List<IkLegPair>();
        public Vector3 TargetPosition;
        [HideInInspector] public Vector3 overrideDir = Vector3.zero;
        [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
        private Waving _waving;
        private IKHarmWiggle _ikHarmWiggle;
        private RigBuilder _rigBuilder;
        private Animator _animator;

        //Parameters
        [TitleGroup("Parameters")]
        [TabGroup("Parameters/A", "General"), SerializeField]
        private bool doAlignTipToLast;
        [TabGroup("Parameters/A", "Raycast"), SerializeField]
        private float _nextTargetRaycastLength = 1;
        [TabGroup("Parameters/A", "Raycast"), SerializeField]
        private float _nextTargetRaycastOriginY = 1;
        [TabGroup("Parameters/A", "Raycast"), SerializeField]
        private float _nextTargetRaycastAnticipation = 1;
        [TabGroup("Parameters/A", "Raycast"), SerializeField]
        private float _checkCoolDown = 0.5f;
        [TabGroup("Parameters/A", "Raycast"), SerializeField]
        private int _raycastTries = 3;
        [TabGroup("Parameters/A", "Raycast"), SerializeField]
        private float _raycastTryWeight = 1;
        [TabGroup("Parameters/A", "Raycast"), SerializeField]
        private LayerMask _raycastLayerMask;


        [TabGroup("Parameters/A", "Metrics"), SerializeField]
        private float _maxLengthBeforeUpdate = 1;
        [TabGroup("Parameters/A", "Metrics"), SerializeField]
        private float _iksDiff = 0.5f;
        [TabGroup("Parameters/A", "Metrics"), SerializeField]
        private float _iksMaxDiff = 0.5f;
        [TabGroup("Parameters/A", "Metrics"), MinMaxSlider(0,5), SerializeField]
        private Vector2 _lengthBeforeUpdateOffSet;
        [TabGroup("Parameters/A", "Metrics"), SerializeField]
        private float _targetHeightTransition = 1;
        [TabGroup("Parameters/A", "Metrics"), SerializeField]
        private AnimationCurve _heightStepCurve;
        [TabGroup("Parameters/A", "Metrics"), MinMaxSlider(0, 20), SerializeField]
        private Vector2 _moveSpeed;
        [TabGroup("Parameters/A", "Metrics"), SerializeField]
        private Vector3 _localUp;
        [TabGroup("Parameters/A", "Metrics"), SerializeField, ReadOnly]
        private Vector3 _up;

        [TabGroup("Parameters/A", "LegMaterialAnim"), SerializeField]
        private float _LegMaterialAnimTime;
        [TabGroup("Parameters/A", "LegMaterialAnim"), SerializeField]
        private int _LegMaterialAnimIterations;

        //Debug
        [TitleGroup("Debug")]
        [TabGroup("Debug/A", "Gizmos"), SerializeField]
        private bool _showGizmos = false;
        [TabGroup("Debug/A", "Gizmos"), ShowIf("_showGizmos"), SerializeField]
        private bool _showGizmosRaycast = false;
        [TabGroup("Debug/A", "Gizmos"), ShowIf("_showGizmos"), SerializeField]
        private bool _showGizmosDir = false;
        [TabGroup("Debug/A", "Gizmos"), ShowIf("_showGizmos"), SerializeField]
        private Color _gizmosRaycastColor = Color.red;
        [TabGroup("Debug/A", "Gizmos"), ShowIf("_showGizmos"), SerializeField]
        private bool _showGizmosMetrics = false;
        [TabGroup("Debug/A", "Gizmos"), ShowIf("_showGizmos"), SerializeField]
        private Color _gizmosMetricsColor = Color.red;

        //Variables
        [TabGroup("Debug/A", "Metrics"), ReadOnly, SerializeField]
        private Vector3 _overideDirDebug;

        //PERFORMANCE
        public int CurrentFrame = 0;
        public int FrameEco = 5;

        // Start is called before the first frame update
        void Start()
        {
            _waving = GetComponent<Waving>();
            _ikHarmWiggle = GetComponent<IKHarmWiggle>();
            _rigBuilder = GetComponent<RigBuilder>();
            _animator = GetComponent<Animator>();
            InitTargetsPos();
        }

        private void InitTargetsPos()
        {
            for (int i = 0; i < _iksLegPairs.Count; i++)
            {
                foreach (Leg leg in _iksLegPairs[i].Legs)
                {
                    leg.LastPos = leg.Target.position;
                    leg.MaxLength = _maxLengthBeforeUpdate + UnityEngine.Random.Range(_lengthBeforeUpdateOffSet.x, _lengthBeforeUpdateOffSet.y);
                    leg.LastPosTarg = leg.Target.position;
                    leg.LastPosTargTotDist = 0;
                    leg.MoveTime = 0;
                    leg.randomizer = UnityEngine.Random.Range(0, 100.0f);
                    
                }
            }
            UpdateTargetsPos();
        }

        private void UpdateTargetsPos()
        {
            for (int i = 0; i < _iksLegPairs.Count; i++)
            {
                foreach (Leg leg in _iksLegPairs[i].Legs)
                {
                    leg.LastPos = leg.Target.position;
                }
            }
        }


        // Update is called once per frame
        void Update()
        {
            _up = transform.forward * _localUp.z + transform.right * _localUp.x + transform.up * _localUp.y;
            for (int i = 0; i < _iksLegPairs.Count; i++)
            {
                foreach (Leg leg in _iksLegPairs[i].Legs)
                {
                    leg.CurrentFrame++;
                    if (leg.CurrentFrame < FrameEco)
                    {
                        continue;
                    }
                    leg.CurrentFrame = 0;
                    if (leg.UseTwoBonesIk && leg.Ik.enabled || !leg.UseTwoBonesIk && leg.ChainIk.enabled)
                    {
                        leg.Target.position = leg.LastPos;
                        //_waving.IkHarmCompensateWave(leg);
                    }

                    if (Vector3.Distance(leg.Target.position, leg.LastPosTarg) > 0.01f)
                    {
                        TransitionLastPos(leg);
                    }
                    else
                    {
                        leg.GroundedDelta = 1;
                    }
                    if (doAlignTipToLast)
                    {
                        leg.Target.rotation = leg.LastBone.transform.parent.rotation;
                    }
                }
            }
            CheckEachIkDistances();
        }

        private void LateUpdate()
        {
            for (int i = 0; i < _iksLegPairs.Count; i++)
            {
                foreach (Leg leg in _iksLegPairs[i].Legs)
                {
                    _ikHarmWiggle.ApplyNoiseToLeg(leg);
                }
            }
        }

        //Anim

        public void TransitionLastPos(Leg leg)
        {
            float deltaP = 1.1f - Vector3.Distance(leg.LastPos, leg.LastPosTarg) / leg.LastPosTargTotDist;
            leg.LastPos = Vector3.Lerp(leg.LastPos, leg.LastPosTarg, Time.deltaTime * leg.MoveTime * deltaP);
            float delta = 1f - Vector3.Distance(leg.LastPos, leg.LastPosTarg) / leg.LastPosTargTotDist;
            float step = _heightStepCurve.Evaluate(delta);
            leg.GroundedDelta = 1-step;
            leg.LegSpeed = step;

            if (delta >= 0.99f && !leg.DidLegJustTouchedGround)
            {
                leg.DidLegJustTouchedGround = true;
                LegTouchedGround(leg);
                StartCoroutine(GroundTouchCoolDown(leg));
            }
            leg.Target.position += _up * _targetHeightTransition * step;
            
        }


        private void LegTouchedGround(Leg leg)
        {
            foreach (int index in leg.ScalesAnimIndexOnGroundTouched)
            {
                leg.tentacle.StartTentacleAnimationByIndex(index);
            }
        }

        IEnumerator GroundTouchCoolDown(Leg leg)
        {
            
            yield return new WaitForSecondsRealtime(1f);
            leg.DidLegJustTouchedGround = false;
        }

        private void CheckEachIkDistances()
        {
            CurrentFrame++;
            if (CurrentFrame < FrameEco)
            {
                return;
            }
            CurrentFrame = 0;
            foreach (IkLegPair legPair in _iksLegPairs)
            {
                if (Vector3.Distance(legPair.Legs[0].LastPos, legPair.Legs[0].TargetPos.position) > legPair.Legs[0].MaxLength*1.5f)
                {
                    legPair.CurrentLeg = legPair.CurrentLeg == 0 ? 1 : 0;
                }
                if (Vector3.Distance(legPair.Legs[1].LastPos, legPair.Legs[1].TargetPos.position) > legPair.Legs[1].MaxLength * 1.5f)
                {
                    legPair.CurrentLeg = legPair.CurrentLeg == 0 ? 1 : 0;
                }
                if (Vector3.Distance(legPair.Legs[0].LastPos, legPair.Legs[0].LastPosTarg) > 1f)
                {
                    continue;
                }
                if (Vector3.Distance(legPair.Legs[1].LastPos, legPair.Legs[1].LastPosTarg) > 1f)
                {
                    continue;
                }
                float distance = Vector3.Distance(legPair.Legs[legPair.CurrentLeg].LastPos, legPair.Legs[legPair.CurrentLeg].TargetPos.position);
                if (distance > legPair.Legs[legPair.CurrentLeg].MaxLength)
                {
                    GetAndApplyNextIkPosition(legPair.Legs[legPair.CurrentLeg], 0);
                    legPair.Legs[legPair.CurrentLeg].MaxLength = _maxLengthBeforeUpdate;
                    legPair.CurrentLeg = legPair.CurrentLeg == 0 ? 1 : 0;
                }
            }
        }

        private void GetAndApplyNextIkPosition(Leg leg, int tryNum)
        {

            Vector3 toAdd = (leg.TargetPos.position - leg.Target.position)* _nextTargetRaycastAnticipation;
            Vector3 pos = leg.TargetPos.position + _up * _nextTargetRaycastOriginY;
            Vector3 projectedPoint = ProjectPositionOnPlane(pos+toAdd, _up, pos);

            Ray ray = new Ray(projectedPoint, -_up);

            Debug.DrawRay(leg.TargetPos.position  + _up * _nextTargetRaycastOriginY, -_up * _nextTargetRaycastLength);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit,_nextTargetRaycastLength, _raycastLayerMask))
            {
                if (leg.UseTwoBonesIk)
                    leg.Ik.enabled = true;
                else
                    leg.ChainIk.enabled = true;
                //leg.Model.SetActive(true);
                leg.LastPosTarg = hit.point;
                leg.LastPosTargTotDist = Vector3.Distance(leg.LastPos, leg.LastPosTarg);
                leg.MoveTime = UnityEngine.Random.Range(_moveSpeed.x, _moveSpeed.y);
                return;
            }
            if (tryNum == _raycastTries)
            {
                if (leg.UseTwoBonesIk)
                    leg.Ik.enabled = false;
                else
                    leg.ChainIk.enabled = false;
                //leg.Model.SetActive(false);
                return;
            }
            GetAndApplyNextIkPosition(leg, tryNum+1);
        }

        [Button, Tooltip("Works only with tentacle script")]
        public void SetupLegs(Transform LegParent)
        {
            for (int i = 0; i < LegParent.childCount; i += 2)
            {
                IkLegPair ikLegPair = new IkLegPair();
                ikLegPair.CurrentLeg = Mathf.FloorToInt((i/2) % 2);
                //FirstLeg
                ikLegPair.Legs[0] = new Leg();
                ikLegPair.Legs[1] = new Leg();
                SetupLeg(ikLegPair.Legs[0], LegParent.GetChild(i));
                SetupLeg(ikLegPair.Legs[1], LegParent.GetChild(i+1));
                _iksLegPairs.Add(ikLegPair);
            }
            if (Application.isPlaying)
            {
                _rigBuilder.Build();
            }
        }

        private void SetupLeg(Leg leg, Transform legParent)
        {
            if (!_rigBuilder || !_animator)
            {
                _rigBuilder = GetComponent<RigBuilder>();
                _animator = GetComponent<Animator>();
            }
            leg.UseTwoBonesIk = false;
            leg.ChainIk = legParent.GetComponent<ChainIKConstraint>();
            leg.Target = legParent.Find("Target");
            leg.TargetPos = legParent.Find("TargetPos");
            leg.LastBone = legParent.GetComponent<Tentacle>().EndTentacleScale;
            leg.FirstBone = legParent.GetComponent<Tentacle>().StartTentacleScale;
            leg.ScalesAnimIndexOnGroundTouched.Add(0);
            Tentacle tentacle = legParent.GetComponent<Tentacle>();
            tentacle.TentRigBuilder = _rigBuilder;
            tentacle.EnemyAnimator = _animator;
            tentacle.leg = leg;
            leg.tentacle = tentacle;

        }

        [Button]
        public void ResetLegs()
        {
            _iksLegPairs.Clear();
        }


        private void OnDrawGizmos()
        {
            if (!_showGizmos)
            {
                return;
            }
            Vector3 up = transform.forward * _localUp.z + transform.right * _localUp.x + transform.up * _localUp.y;
            for (int i = 0; i < _iksLegPairs.Count; i++)
            {
                foreach (Leg leg in _iksLegPairs[i].Legs)
                {
                    if (_showGizmosRaycast)
                    {
                        Gizmos.color = _gizmosRaycastColor;
                        Vector3 toAdd = (leg.TargetPos.position - leg.Target.position) * _nextTargetRaycastAnticipation;

                        Vector3 pos = leg.TargetPos.position + up * _nextTargetRaycastOriginY;
                        Vector3 projectedPoint = ProjectPositionOnPlane(pos+toAdd, up, pos);
                        Gizmos.DrawLine(pos, pos - up * _nextTargetRaycastLength);
                        Gizmos.color = Color.white;
                        Gizmos.DrawLine(projectedPoint, projectedPoint - up * _nextTargetRaycastLength);
                        Gizmos.DrawWireCube(leg.TargetPos.position, Vector3.one * 3);
                        if (_showGizmosDir)
                        {
                            Gizmos.DrawLine(leg.Target.position + up * 2, leg.Target.position + up * 2 + overrideDir);
                        }
                    }
                    if (_showGizmosMetrics)
                    {
                        Gizmos.color = _gizmosMetricsColor;
                        if (Application.isPlaying)
                        {
                            Gizmos.color = Color.green;
                            if (Vector3.Distance(leg.LastPos, leg.LastPosTarg) > 1f)
                            {
                                Gizmos.color = Color.magenta;
                            }
                            Gizmos.DrawWireSphere(leg.Target.position, leg.MaxLength);
                            continue;
                        }
                        Gizmos.DrawWireSphere(leg.Target.position, _maxLengthBeforeUpdate + _lengthBeforeUpdateOffSet.y);
                    }
                    Gizmos.color = _gizmosMetricsColor;
                    Gizmos.DrawWireCube(leg.Target.position + up * _targetHeightTransition, new Vector3(0.1f, 0, 0.1f));
                }
            }
        }

        private Vector3 ProjectPositionOnPlane(Vector3 position, Vector3 normal, Vector3 planePoint)
        {
            Plane plane = new Plane(normal, planePoint);
            return plane.ClosestPointOnPlane(position);
        }

    }
}
