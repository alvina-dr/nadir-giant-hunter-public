using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName ="EnemySpawnerData", menuName = "ScriptableObjects/EnemySpawnerData")]
public class EnemySpawnerData : ScriptableObject
{
    public float SpawnCircleWidth;
    public GameObject EnemyPrefab;

    public float SpawnCycle;
    public int MaxEnemies;

}
