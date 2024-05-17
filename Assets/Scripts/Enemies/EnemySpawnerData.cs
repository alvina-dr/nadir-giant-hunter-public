using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

[Serializable]
public class SpawningTimer
{
    [Unit(Units.Second), PropertyTooltip("Starting point of the spawning timer in sec since the start of the game")]
    public float TimerStart;

    public int MaxEnemiesSpawned;
    [Unit(Units.Second), PropertyTooltip("Time between two monster spawn")]
    public float SpawnCoolDown;
    [PropertyTooltip("Is there only one monster spawned per line")]
    public bool DoSpawnOnlyOnePerLine;

}


[CreateAssetMenu(fileName ="EnemySpawnerData", menuName = "ScriptableObjects/EnemySpawnerData")]
public class EnemySpawnerData : ScriptableObject
{
    public GameObject EnemyPrefab;
    
    public List<SpawningTimer> spawningTimers = new List<SpawningTimer>();
}
