using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Profiling;

namespace Enemies
{
    //BURST JOBS
    [BurstCompile]
    public struct CalculTentacleScaleAnim : IJobParallelFor
    {
        [Unity.Collections.ReadOnly]
        public NativeArray<float3> tentsPos;
        [Unity.Collections.ReadOnly]
        public NativeArray<float3> nextTentsPos;

        [WriteOnly]
        public NativeArray<Quaternion> newRotations;

        public void Execute(int index)
        {
            Vector3 tentPos = tentsPos[index];
            Vector3 nextTentPos = nextTentsPos[index];
            

            Quaternion tentParRot = Quaternion.identity;
            Vector3 dir = tentPos - nextTentPos;
            if (dir.x != 0 || dir.y != 0 || dir.z != 0)
            {
                Debug.Log(dir);
                tentParRot = Quaternion.LookRotation(tentPos - nextTentPos, Vector3.up);
            }
            newRotations[index] = tentParRot;
        }
    }

    public class EnemyAnimCalculator : MonoBehaviour
    {
        //public List<EnemyBuilder> builders = new List<EnemyBuilder>();

        public bool isNotTentaclesScales;
        public List<Tentacle> tentacles = new List<Tentacle>();
        private Transform[] tentacleScales;
        private Transform[] tentacleScalesMid;

        NativeArray<Quaternion> rotations;
        NativeArray<float3> tentsPos;
        NativeArray<float3> nextTentsPos;
        TransformAccessArray trTentacleScales;
        TransformAccessArray trTentacleScalesNext;

        // Start is called before the first frame update
        void Start()
        {
            /*foreach (EnemyBuilder builder in builders)
            {
                foreach(Tentacle tentacle in builder.Tentacles)
                {
                    tentacles.Add(tentacle);
                }
            }*/
            int count=0;
            foreach (Tentacle tentacle in tentacles)
            {
                count += tentacle.tentacleScales.Length;
                if (isNotTentaclesScales)
                {
                    count += tentacle.bones.Length;
                }
            }
            tentacleScales = new Transform[count];
            tentacleScalesMid = new Transform[count];
            int tentScCount=0;
            foreach (Tentacle tentacle in tentacles)
            {
                if (!tentacle.isNotTentacleScales)
                {
                    foreach (TentacleScale tentacleScale in tentacle.tentacleScales)
                    {
                        tentacleScales[tentScCount] = tentacleScale.transform;
                        tentacleScalesMid[tentScCount] = tentacleScale.TentacleModel.transform;
                        tentScCount++;
                    }
                }
                else
                {
                    foreach (Transform bone in tentacle.bones)
                    {
                        tentacleScales[tentScCount] = bone;
                        tentacleScalesMid[tentScCount] = bone;
                        tentScCount++;
                    }
                }

            }

            rotations = new NativeArray<Quaternion>(count, Allocator.Persistent);
            tentsPos = new NativeArray<float3>(count, Allocator.Persistent);
            nextTentsPos = new NativeArray<float3>(count, Allocator.Persistent);
        }

        // Update is called once per frame
        void LateUpdate()
        {
            CalculateScalesTransforms();
        }

        private void CalculateScalesTransforms()
        {
            Profiler.BeginSample(":::TentacleJOB");
            Profiler.BeginSample("BaseSetup");
            
            for (int i = 0; i < tentacleScales.Length; i++)
            {
                tentsPos[i] = tentacleScales[i].position;
                nextTentsPos[i] = tentacleScales[i].parent.position;
            }
            Profiler.EndSample();


            Profiler.BeginSample("Job");

            //Rotation
            CalculTentacleScaleAnim tentacleScaleJob = new CalculTentacleScaleAnim()
            {
                tentsPos = tentsPos,
                nextTentsPos= nextTentsPos,
                newRotations = rotations,
            };

            JobHandle tentacleScaleJobHandle = tentacleScaleJob.Schedule(tentacleScales.Length, 1);

            tentacleScaleJobHandle.Complete();
            Profiler.EndSample();

            Profiler.BeginSample("Apply");
            for (int i = 0; i < tentacleScales.Length; i++)
            {
                tentacleScales[i].rotation = tentacleScaleJob.newRotations[i];
                tentacleScales[i].position = tentsPos[i];
            }
            Profiler.EndSample();

            Profiler.EndSample();
        }
    }
}
