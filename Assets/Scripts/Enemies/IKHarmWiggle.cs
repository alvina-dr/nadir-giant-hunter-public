using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
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
        public float NoiseScale = 0f;
        public float LegSpeedInfluence = 0f;
        public float NoiseSpeed = 0f;
        public float RandomMult = 0f;
        public int BoneIndex = 0;

        //Performance
        public int CurrentFrame = 0;
        public int FrameEco = 3;
        private bool doUpdate = true;

        private List<LegAndLegTrans> _legAndLegTrans = new List<LegAndLegTrans>(); 

        //Accumulation of Transformation applied to bone
        private class LegTransformation
        {
            public int LegBoneCount = 0;
            public float cummuledDistance = 0;
            public List<Vector3> PreTransformations = new List<Vector3>();
            public List<Vector3> Transformations = new List<Vector3>();
        }
        private class LegAndLegTrans
        {
            public Leg leg;
            public LegTransformation legTransformation;
        }

        
        void Start()
        {
            
        }

        void Update()
        {
            if (doUpdate)
            {
                doUpdate = false;
            }
            CurrentFrame++;
            if (CurrentFrame >= FrameEco)
            {
                CurrentFrame = 0;
                doUpdate = true;
            }
        }

        public void ApplyNoiseToLeg(Leg leg)
        {
            LegAndLegTrans legAndLegTran = _legAndLegTrans.Find(x => x.leg == leg);
            if (legAndLegTran == null)
            {
                LegTransformation legTransformation = new LegTransformation();
                GetLegBoneNum(leg.FirstBone.transform, legTransformation, leg);
                legTransformation.Transformations = new List<Vector3>();
                legTransformation.PreTransformations = new List<Vector3>();
                for (int i = 0; i < legTransformation.LegBoneCount; i++)
                {
                    legTransformation.Transformations.Add(Vector3.zero);
                    legTransformation.PreTransformations.Add(Vector3.zero);
                }
                legAndLegTran = new LegAndLegTrans();
                legAndLegTran.legTransformation = legTransformation;
                legAndLegTran.leg = leg;
                _legAndLegTrans.Add(legAndLegTran);
            }

            toGui = "";
            ApplyNoiseToTransformAndChild(leg.FirstBone.transform, legAndLegTran.legTransformation, legAndLegTran.leg, 0);
        }

        private void GetLegBoneNum(Transform legBone, LegTransformation legTransformation, Leg leg)
        {
            legTransformation.LegBoneCount++;
            if (legBone.childCount > 0 && legBone.GetChild(BoneIndex).gameObject != leg.LastBone)
            {
                GetLegBoneNum(legBone.GetChild(BoneIndex), legTransformation, leg);
            }
        }

        private void ApplyNoiseToTransformAndChild(Transform transformed, LegTransformation legTransformation, Leg leg, int index)
        {
            if (doUpdate)
            {
                UseLastFramePos(transformed, legTransformation, leg, index);
            }
            else
            {
                CompensanteTransformation(transformed, legTransformation, index);
                AddNoiseTransformation(transformed, legTransformation, leg, index);
            }
            if (transformed.childCount > 0 && transformed.GetChild(BoneIndex).gameObject != leg.LastBone)
            {
                index++;
                ApplyNoiseToTransformAndChild(transformed.GetChild(BoneIndex), legTransformation, leg, index);
            }
        }

        private void CompensanteTransformation(Transform transformed, LegTransformation legTransformation, int index)
        {
            if (index != 0)
            {
                transformed.position -= legTransformation.Transformations[index - 1];
            }

            legTransformation.cummuledDistance += transformed.localPosition.magnitude;
            legTransformation.PreTransformations[index] = transformed.position;
        }

        private void AddNoiseTransformation(Transform transformed, LegTransformation legTransformation, Leg leg, int index)
        {
            int currentBoneNum = index;
            float delta = currentBoneNum / (float)legTransformation.LegBoneCount;
            float noiseInfluence = Mathf.Lerp(NoiseInflUngrounded.Evaluate(delta), NoiseInflGrounded.Evaluate(delta), leg.GroundedDelta);
            deltaGrounded = leg.GroundedDelta;
            Vector3 transformation = Vector3.zero;
            if (currentBoneNum > 1)
            {
                float noiseTime = 0.01f*(Time.time * NoiseSpeed + leg.randomizer * RandomMult);
                float positionInfl = 0.2f;
                transformation.x = (NoiseScale * 0.01f) * (Mathf.PerlinNoise(transformed.position.z * positionInfl + noiseTime, transformed.position.y * positionInfl + noiseTime) -0.5f);
                transformation.y = (NoiseScale * 0.01f) * (Mathf.PerlinNoise(transformed.position.z * positionInfl + noiseTime + 1000, transformed.position.y * positionInfl + noiseTime + 1000) - 0.5f);
                transformation.z = (NoiseScale * 0.01f) * (Mathf.PerlinNoise(transformed.position.z * positionInfl + noiseTime + 2000, transformed.position.y * positionInfl + noiseTime + 2000) - 0.5f);
                /*transformation.x = Mathf.PerlinNoise1D(NoiseTimeInfluenceMult * (Time.time * NoiseSpeed + legTransformation.cummuledDistance + leg.randomizer * RandomMult)) -0.5f;
                transformation.y = Mathf.PerlinNoise1D(NoiseTimeInfluenceMult * (Time.time * NoiseSpeed + legTransformation.cummuledDistance+1000 + leg.randomizer * RandomMult)) - 0.5f;
                transformation.z = Mathf.PerlinNoise1D(NoiseTimeInfluenceMult * (Time.time * NoiseSpeed + legTransformation.cummuledDistance+2000 + leg.randomizer * RandomMult)) - 0.5f;*/
                transformation *= noiseInfluence * NoiseInfluenceMult;
                Vector3 oldPos = transformed.position;
                Vector3 parentUnTrans = transformed.parent.position - legTransformation.Transformations[index - 1];
                transformed.position += transformation;
                Vector3 planeUp = parentUnTrans - oldPos;
                Plane plane = new Plane(planeUp, oldPos);
                transformed.position = plane.ClosestPointOnPlane(transformed.position);
                transformation = transformed.position - oldPos;
            }
            legTransformation.Transformations[index] = transformation;
            //toGui += currentBoneNum + " : " + noiseInfluence + " | \n   "+ transformation +"\n";
        }

        private void UseLastFramePos(Transform transformed, LegTransformation legTransformation, Leg leg, int index)
        {
            transformed.position = legTransformation.PreTransformations[index] + legTransformation.Transformations[index];
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

