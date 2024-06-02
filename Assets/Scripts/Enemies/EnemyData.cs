using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Enemies
{
    [CreateAssetMenu(fileName = "EnemyData", menuName ="ScriptableObjects/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        public float MoveSpeed;
        public float DistanceToGround;

    }
}
