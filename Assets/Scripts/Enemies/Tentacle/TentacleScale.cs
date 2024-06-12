using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Profiling;

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
        public GameObject TentacleModel;
        [TabGroup("Components")]
        public List<Renderer> TentacleRenderers = new List<Renderer>();

        private Vector3 _baseScale;
        [TabGroup("Parameter")]
        public Vector3 ScaleMaxAdded;
        [TabGroup("Parameter"), Range(0, 1)]
        public float ScaleDelta;

        [TabGroup("Parameter")]
        public Vector3 RotationAxisSpeed;
        [TabGroup("Parameter"), Range(-1,1)]
        public float RotationSpeedDelta;

        private Vector3 _baseRotation;
        [TabGroup("Parameter")]
        public Vector3 RotationMax;
        [TabGroup("Parameter"), Range(0,1)]
        public float RotationDelta;

        [TabGroup("Parameter")]
        public string MatParameterName;
        [TabGroup("Parameter"), Range(0,1)]
        public float MatParameterDelta;

        [TabGroup("Parameter")]
        public string MatColorName;
        [TabGroup("Parameter")]
        public Color MatBaseColor;
        [TabGroup("Parameter")]
        public Color MatSecColor;
        [TabGroup("Parameter"), Range(0,1)]
        public float MatColorDelta;

        [TabGroup("Debug"), SerializeField, ReadOnly]
        private float _GlobalDelta;
        [TabGroup("Debug"), SerializeField, Range(0, 1)]
        private float _DebugDelta;

        //PERFORMANCE
        [TabGroup("Debug")]
        public int CurrentFrame = 0;
        [TabGroup("Debug")]
        public int FrameEco = 5;

        private float lastAngle;

        private void Start()
        {
            _baseScale = TentacleModel.transform.localScale;
            _baseRotation = TentacleModel.transform.localEulerAngles;
        }

        private void Update()
        {
        
        }

        private void LateUpdate()
        {
            /*Profiler.BeginSample("TentacleScaleLateUpd");
            CurrentFrame++;
            if (CurrentFrame < FrameEco)
            {
                return;
            }
            CurrentFrame = 0;
            ApplyScale();
            ApplyRotation();
            ApplyOpening();
            ApplyMatParam();
            ApplyMatColor();
            _GlobalDelta = 0;
            ScaleDelta = 0;
            OpeningDelta = 0;
            RotationDelta = 0;
            MatParameterDelta = 0;
            MatColorDelta = 0;

            
            Profiler.EndSample();*/
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
            Profiler.BeginSample("Scale");
            TentacleModel.transform.localScale = Vector3.Lerp(_baseScale, _baseScale + ScaleMaxAdded, ScaleDelta + _GlobalDelta + _DebugDelta);
            Profiler.EndSample();
        }

        private void ApplyRotation()
        {
            Profiler.BeginSample("ApplyRotation2");
            if ((transform.parent.position - transform.position)!=Vector3.zero)
            {
                TentacleModel.transform.rotation = Quaternion.LookRotation(transform.position - transform.parent.position, Vector3.up);
                //TentacleRenderer.transform.localEulerAngles = Vector3.Lerp(_baseRotation, _baseRotation+RotationMax, RotationDelta + _GlobalDelta + _DebugDelta);
            }
            Profiler.EndSample();

            //TentacleRenderer.transform.localRotation *= Quaternion.AngleAxis(lastAngle + RotationSpeedDelta, Vector3.up);
            //lastAngle = lastAngle + RotationSpeedDelta;
            //TentacleRenderer.transform.localEulerAngles += RotationAxisSpeed * ;
        }

        private void ApplyMatParam()
        {
            Profiler.BeginSample("ApplyMatParam");
            foreach (Renderer tentacleRenderer in TentacleRenderers)
            {
                tentacleRenderer.material.SetFloat(MatParameterName, MatParameterDelta + _GlobalDelta + _DebugDelta);
            }
            Profiler.EndSample();
        }

        private void ApplyMatColor()
        {
            Profiler.BeginSample("ApplyMatColor");
            if (MatColorName == "")
                return;
            foreach (Renderer tentacleRenderer in TentacleRenderers)
            {
                tentacleRenderer.material.SetColor(MatColorName, Color.Lerp(MatBaseColor, MatSecColor, MatColorDelta + _GlobalDelta + _DebugDelta));
            }
            Profiler.EndSample();
        }

    }
}

