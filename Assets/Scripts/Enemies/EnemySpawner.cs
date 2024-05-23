using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [TitleGroup("Components")]
    public EnemySpawnerData SpawnerData;
    [SerializeField]
    private Transform _enemiesParent;

    [SerializeField]
    private List<Transform> _enemiesSpawnPoints = new List<Transform>();
    [SerializeField, Sirenix.OdinInspector.ReadOnly]
    private List<GameObject> _enemies = new List<GameObject>();

    private int _currentSpawningTimerIndex;
    public int NumTotalEnemy = 0;

    void Start()
    {
        _currentSpawningTimerIndex = 0;

        if (SpawnerData.spawningTimers[_currentSpawningTimerIndex].DoSpawnOnlyOnePerLine)
        {
            foreach (var item in _enemiesSpawnPoints)
            {
                _enemies.Add(null);
            }
        }
        StartCoroutine(SpawnCycle());
        StartCoroutine(WaitForNextSpawningTimer());
    }

    IEnumerator WaitForNextSpawningTimer()
    {
        //if the Spawning Timer isn't the last
        if (_currentSpawningTimerIndex + 1 != SpawnerData.spawningTimers.Count)
        {
            //schedule the set toward the next spawning timer 
            SpawningTimer nextSpawningTimer = SpawnerData.spawningTimers[_currentSpawningTimerIndex+1];
            Debug.Log("will wait : " + (nextSpawningTimer.TimerStart - SpawnerData.spawningTimers[_currentSpawningTimerIndex].TimerStart));
            yield return new WaitForSecondsRealtime(nextSpawningTimer.TimerStart - SpawnerData.spawningTimers[_currentSpawningTimerIndex].TimerStart);
            Debug.Log("Incremented");
            _currentSpawningTimerIndex++;
            StartCoroutine(WaitForNextSpawningTimer());
        }
    }

    IEnumerator SpawnCycle()
    {
        //Spawn enemies relative to the spawn cooldown
        while (_currentSpawningTimerIndex != SpawnerData.spawningTimers.Count) {
            SpawnNewEnemy();
            yield return new WaitForSeconds(SpawnerData.spawningTimers[_currentSpawningTimerIndex].SpawnCoolDown);
        }
    }

    public void SpawnNewEnemy()
    {
        int SpawnPointIndex = Random.Range(0, _enemiesSpawnPoints.Count);
        bool hasTooMuchEnemies = GetAliveEnemiesNum() >= SpawnerData.spawningTimers[_currentSpawningTimerIndex].MaxEnemiesSpawned;
        //Spawn an enemy if there's not too much enemies and, if doSpawnOnlyOnePerLine and line isn't occupied
        if (hasTooMuchEnemies || (SpawnerData.spawningTimers[_currentSpawningTimerIndex].DoSpawnOnlyOnePerLine && _enemies[SpawnPointIndex] != null))
        {
            return;
        }
        GameObject enemy = Instantiate(SpawnerData.EnemyPrefab);
        enemy.transform.parent = _enemiesParent;

        //Set enemy's transform
        Transform pos = _enemiesSpawnPoints[SpawnPointIndex];
        enemy.transform.position = pos.position;
        enemy.transform.rotation = pos.rotation;

        //Depending on doSpawnOnlyOnePerLine, set the "_enemies" list
        if (SpawnerData.spawningTimers[_currentSpawningTimerIndex].DoSpawnOnlyOnePerLine)
        {
            _enemies[SpawnPointIndex] = enemy;
        }
        else
        {
            _enemies.Add(enemy);
        }
    }

    public int GetAliveEnemiesNum()
    {
        //Maybe TO UPDATE due to enemies life (and corpses)
        int enemiesNum = 0;
        foreach (GameObject enemy in _enemies)
        {
            if (enemy != null)
            {
                enemiesNum++;
            }
        }
        return enemiesNum;
    }

    private void OnDrawGizmosSelected()
    {
        foreach(Transform enemySpawnPoint in _enemiesSpawnPoints)
        {
            Gizmos.DrawWireSphere(enemySpawnPoint.position, 5);
            Gizmos.DrawLine(enemySpawnPoint.position, enemySpawnPoint.position+Vector3.up*500);
        }
    }
}
