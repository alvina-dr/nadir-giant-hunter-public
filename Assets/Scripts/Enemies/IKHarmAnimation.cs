using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Sirenix.OdinInspector;
using DG.Tweening;
using System;
using static ak.wwise.core;

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
        [SerializeField] private List<IkLegPair> _iksLegPairs = new List<IkLegPair>();
        public Vector3 TargetPosition;
        [HideInInspector] public Vector3 overrideDir = Vector3.zero;


        //Parameters
        [TitleGroup("Parameters")]
        [TabGroup("Parameters/A", "General"), SerializeField]
        private bool doAlignTipToLast;
        [TabGroup("Parameters/A", "Raycast"), SerializeField]
        private float _nextTargetRaycastLength = 1;
        [TabGroup("Parameters/A", "Raycast"), SerializeField]
        private float _nextTargetRaycastOriginY = 1;
        [TabGroup("Parameters/A", "Raycast"), SerializeField]
        private float _nextTargetRaycastY = 1;
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
        [TabGroup("Parameters/A", "Metrics"), MinMaxSlider(0, 50), SerializeField]
        private Vector2 _moveSpeed;
        [TabGroup("Parameters/A", "Metrics"), SerializeField]
        private Vector3 _localUp;
        [TabGroup("Parameters/A", "Metrics"), SerializeField, ReadOnly]
        private Vector3 _up;

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

        // Start is called before the first frame update
        void Start()
        {
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

                    if (leg.UseTwoBonesIk && leg.Ik.enabled || !leg.UseTwoBonesIk && leg.ChainIk.enabled)
                    {
                        leg.Target.position = leg.LastPos;
                    }

                    if (Vector3.Distance(leg.LastPos, leg.LastPosTarg) > 1f)
                    {
                        TransitionLastPos(leg);
                    }
                    if (doAlignTipToLast)
                    {
                        leg.Target.rotation = leg.LastBone.transform.parent.rotation;
                    }
                }
            }
            CheckEachIkDistances();
        }

        //Anim

        public void TransitionLastPos(Leg leg)
        {
            leg.LastPos = Vector3.Lerp(leg.LastPos, leg.LastPosTarg, Time.deltaTime * leg.MoveTime);
            float delta = 1 - Vector3.Distance(leg.LastPos, leg.LastPosTarg) / leg.LastPosTargTotDist;
            float step = -(Mathf.Pow(2 * delta - 1, 2)) + 1;
            leg.Target.position += -_up * _targetHeightTransition * step;
        }



        private void CheckEachIkDistances()
        {
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
            Vector3 up = transform.forward * _up.z + transform.right * _up.x + transform.up * _up.y;
            Ray ray = new Ray(leg.TargetPos.position + toAdd + up * _nextTargetRaycastOriginY, -up);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit,_nextTargetRaycastLength, _raycastLayerMask))
            {
                Debug.Log("1");
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
                        Vector3 toAdd = (leg.TargetPos.position - leg.Target.position);
                        Vector3 pos = leg.TargetPos.position + up * _nextTargetRaycastOriginY;
                        Gizmos.DrawLine(pos, pos + -up * _nextTargetRaycastY);
                        Gizmos.color = Color.white;
                        Gizmos.DrawLine(pos+ toAdd, pos + toAdd + -up * _nextTargetRaycastY);
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

    }
}
