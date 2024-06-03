using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Enemies
{
    public class TentacleScale : MonoBehaviour
    {
        public enum DeltaType
        {
            Global,
            Scale,
            RotationSpeed,
            Rotation,
            MatParam,
            MatColor,
        }

        [TabGroup("Components")]
        public Renderer TentacleRenderer;

        private Vector3 _baseScale;
        public Vector3 ScaleMaxAdded;
        [Range(0,1)]
        public float ScaleDelta;

        public Vector3 RotationAxisSpeed;
        [Range(-1,1)]
        public float RotationSpeedDelta;

        private Vector3 _baseRotation;
        public Vector3 RotationMax;
        [Range(0,1)]
        public float RotationDelta;

        public string MatParameterName;
        [Range(0,1)]
        public float MatParameterDelta;

        public string MatColorName;
        public Color MatBaseColor;
        public Color MatSecColor;
        [Range(0,1)]
        public float MatColorDelta;

        [SerializeField, ReadOnly]
        private float _GlobalDelta;
        [SerializeField, Range(0, 1)]
        private float _DebugDelta;

        private float lastAngle;

        private void Start()
        {
            _baseScale = TentacleRenderer.transform.localScale;
            _baseRotation = TentacleRenderer.transform.localEulerAngles;
        }

        private void Update()
        {
        
        }

        private void LateUpdate()
        {
            ApplyScale();
            ApplyRotation();
            ApplyMatParam();
            ApplyMatColor();
            _GlobalDelta = 0;
            ScaleDelta = 0;
            RotationDelta = 0;
            MatParameterDelta = 0;
            MatColorDelta = 0;
        }

        public void AddGlobalDelta(float delta)
        {
            _GlobalDelta += delta;
        }

        public void AddDelta(float delta, DeltaType deltaType)
        {
            switch (deltaType)
            {
                case DeltaType.Global:
                {
                    AddGlobalDelta(delta); 
                    break;
                }
                case DeltaType.Scale:
                {
                    ScaleDelta += delta;
                    break;
                }
                case DeltaType.RotationSpeed:
                {
                    RotationSpeedDelta += delta;
                    break;
                }
                case DeltaType.Rotation:
                {
                    RotationDelta += delta;
                    break;
                }
                case DeltaType.MatParam:
                {
                    MatParameterDelta += delta;
                    break;
                }
                case DeltaType.MatColor:
                {
                    MatColorDelta += delta;
                    break;
                }
            }
        }


        private void ApplyScale()
        {
            TentacleRenderer.transform.localScale = Vector3.Lerp(_baseScale, _baseScale + ScaleMaxAdded, ScaleDelta + _GlobalDelta + _DebugDelta);
        }

        private void ApplyRotation()
        {
            TentacleRenderer.transform.rotation = Quaternion.EulerRotation(_baseRotation);
            if (transform.parent != null && transform.parent.name != "Start" && (transform.parent.position - transform.position)!=Vector3.zero)
            {
                TentacleRenderer.transform.rotation = Quaternion.LookRotation(transform.parent.position - transform.position, Vector3.up);
                TentacleRenderer.transform.localRotation *= Quaternion.AngleAxis(-90, Vector3.right);
                //TentacleRenderer.transform.localEulerAngles = Vector3.Lerp(_baseRotation, _baseRotation+RotationMax, RotationDelta + _GlobalDelta + _DebugDelta);
            }
            //TentacleRenderer.transform.localRotation *= Quaternion.AngleAxis(lastAngle + RotationSpeedDelta, Vector3.up);
            //lastAngle = lastAngle + RotationSpeedDelta;
            //TentacleRenderer.transform.localEulerAngles += RotationAxisSpeed * ;
        }

        private void ApplyMatParam()
        {
            if (MatParameterName == "")
                return;
            TentacleRenderer.material.SetFloat(MatParameterName, MatParameterDelta + _GlobalDelta + _DebugDelta);
        }

        private void ApplyMatColor()
        {
            if (MatColorName == "")
                return;
            TentacleRenderer.material.SetColor(MatColorName, Color.Lerp(MatBaseColor, MatSecColor, MatColorDelta + _GlobalDelta + _DebugDelta));
        }

    }
}

