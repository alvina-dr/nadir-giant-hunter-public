using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

namespace Enemies
{
    public class IKHarmWiggle : MonoBehaviour
    {
        private string toGui = "";
        public AnimationCurve NoiseInflGrounded;
        public AnimationCurve NoiseInflUngrounded;
        [PropertyRange(0f, 1f)]
        public float deltaGrounded;
        public float NoiseInfluenceMult = 0f;
        public float NoiseTimeInfluenceMult = 0f;
        public float LegSpeedInfluence = 0f;
        public float NoiseSpeed = 0f;
        public float RandomMult = 0f;
        public int BoneIndex = 0;

        //Accumulation of Transformation applied to bone
        private class LegTransformation
        {
            public int LegBoneCount = 0;
            public float cummuledDistance = 0;
            public List<Vector3> PreTransformations = new List<Vector3>();
            public List<Vector3> Transformations = new List<Vector3>();
        }

        
        void Start()
        {
            
        }

        void Update()
        {

        }

        public void ApplyNoiseToLeg(Leg leg)
        {
            LegTransformation legTransformation = new LegTransformation();
            GetLegBoneNum(leg.FirstBone.transform, legTransformation, leg);
            toGui = "";
            ApplyNoiseToTransformAndChild(leg.FirstBone.transform, legTransformation, leg);
        }

        private void GetLegBoneNum(Transform legBone, LegTransformation legTransformation, Leg leg)
        {
            legTransformation.LegBoneCount++;
            if (legBone.childCount > 0 && legBone.GetChild(BoneIndex).gameObject != leg.LastBone)
            {
                GetLegBoneNum(legBone.GetChild(BoneIndex), legTransformation, leg);
            }
        }

        private void ApplyNoiseToTransformAndChild(Transform transformed, LegTransformation legTransformation, Leg leg)
        {
            CompensanteTransformation(transformed, legTransformation);
            AddNoiseTransformation(transformed, legTransformation, leg);
            if (transformed.childCount > 0 && transformed.GetChild(BoneIndex).gameObject != leg.LastBone)
            {
                ApplyNoiseToTransformAndChild(transformed.GetChild(BoneIndex), legTransformation, leg);
            }
        }

        private void CompensanteTransformation(Transform transformed, LegTransformation legTransformation)
        {
            if (legTransformation.PreTransformations.Count != 0)
            {
                transformed.position -= legTransformation.Transformations[legTransformation.Transformations.Count-1];
            }

            legTransformation.cummuledDistance += transformed.localPosition.magnitude;
            legTransformation.PreTransformations.Add(transformed.position);
        }

        private void AddNoiseTransformation(Transform transformed, LegTransformation legTransformation, Leg leg)
        {
            int currentBoneNum = legTransformation.PreTransformations.Count;
            float delta = currentBoneNum / (float)legTransformation.LegBoneCount;
            float noiseInfluence = Mathf.Lerp(NoiseInflUngrounded.Evaluate(delta), NoiseInflGrounded.Evaluate(delta), leg.GroundedDelta);
            deltaGrounded = leg.GroundedDelta;
            Vector3 transformation = Vector3.zero;
            if (currentBoneNum > 1)
            {
                transformation.x = Mathf.PerlinNoise1D(NoiseTimeInfluenceMult * (Time.time * NoiseSpeed + legTransformation.cummuledDistance + leg.randomizer * RandomMult)) -0.5f;
                transformation.y = Mathf.PerlinNoise1D(NoiseTimeInfluenceMult * (Time.time * NoiseSpeed + legTransformation.cummuledDistance+1000 + leg.randomizer * RandomMult)) - 0.5f;
                transformation.z = Mathf.PerlinNoise1D(NoiseTimeInfluenceMult * (Time.time * NoiseSpeed + legTransformation.cummuledDistance+2000 + leg.randomizer * RandomMult)) - 0.5f;
                transformation *= noiseInfluence * NoiseInfluenceMult;
                Vector3 oldPos = transformed.position;
                Vector3 parentUnTrans = transformed.parent.position - legTransformation.Transformations[legTransformation.Transformations.Count - 1];
                transformed.position += transformation;
                Vector3 planeUp = parentUnTrans - oldPos;
                Plane plane = new Plane(planeUp, oldPos);
                transformed.position = plane.ClosestPointOnPlane(transformed.position);
                transformation = transformed.position - oldPos;
            }
            legTransformation.Transformations.Add(transformation);
            //toGui += currentBoneNum + " : " + noiseInfluence + " | \n   "+ transformation +"\n";
        }

        /*private void OnGUI()
        {
            GUILayout.Box($"NoiseInfluenceMult : \n{NoiseInfluenceMult}");
            NoiseInfluenceMult = GUILayout.HorizontalSlider(NoiseInfluenceMult, 0, 100000.0f);
            GUILayout.Box($"NoiseTimeInfluenceMult : \n{NoiseTimeInfluenceMult}");
            NoiseTimeInfluenceMult = GUILayout.HorizontalSlider(NoiseTimeInfluenceMult, 0, 1.0f);
            GUILayout.Box($"NoiseSpeed : \n{NoiseSpeed}");
            NoiseSpeed = GUILayout.HorizontalSlider(NoiseSpeed, 0, 10.0f);
            GUILayout.Box($"NoiseInfluenceMult : \n{RandomMult}");
            RandomMult = GUILayout.HorizontalSlider(RandomMult, 0, 10000);
            GUIStyle style = GUI.skin.box;
            style.alignment = TextAnchor.MiddleLeft;
            GUILayout.Box($"Noise Transformation : \n{toGui}", style);

        }*/
    }
}

