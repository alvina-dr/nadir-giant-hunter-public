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
            if (dir.magnitude != 0)
            {
                tentParRot = Quaternion.LookRotation(tentPos - nextTentPos, Vector3.up);
            }
            newRotations[index] = tentParRot;
        }
    }

    public class EnemyAnimCalculator : MonoBehaviour
    {
        public List<EnemyBuilder> builders = new List<EnemyBuilder>();

        private List<Tentacle> tentacles = new List<Tentacle>();
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
            foreach (EnemyBuilder builder in builders)
            {
                foreach(Tentacle tentacle in builder.Tentacles)
                {
                    tentacles.Add(tentacle);
                }
            }
            int count=0;
            foreach (Tentacle tentacle in tentacles)
            {
                count += tentacle.tentacleScales.Length;
            }
            tentacleScales = new Transform[count];
            tentacleScalesMid = new Transform[count];
            int tentScCount=0;
            foreach (Tentacle tentacle in tentacles)
            {
                foreach (TentacleScale tentacleScale in tentacle.tentacleScales)
                {
                    tentacleScales[tentScCount] = tentacleScale.transform;
                    tentacleScalesMid[tentScCount] = tentacleScale.TentacleMid.transform;
                    tentScCount++;
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
                tentacleScalesMid[i].rotation = tentacleScaleJob.newRotations[i];
            }
            Profiler.EndSample();

            Profiler.EndSample();
        }
    }
}
