using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Enemies;

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
    public List<EnemyMovement> EnemyList;

    private int _currentSpawningTimerIndex;

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

    private void Update()
    {
        if (EnemyList.Count > 0)
        {
            GPCtrl.Instance.UICtrl.MonsterHighIndicator.ShowIndicatorAt(EnemyList[0].EnemyWeakSpotManagement.WeakSpotList[0].transform.position);
        }
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
        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
        enemy.transform.parent = _enemiesParent;

        //Set enemy's transform
        Transform pos = _enemiesSpawnPoints[SpawnPointIndex];
        enemy.transform.position = pos.position;
        enemy.transform.rotation = pos.rotation;

        //difficulty choice
        switch (DataHolder.Instance.CurrentDifficulty)
        {
            case DataHolder.DifficultyMode.Easy:
                enemyMovement.Data = GPCtrl.Instance.EasyEnemyData;
                break;
            case DataHolder.DifficultyMode.Normal:
                enemyMovement.Data = GPCtrl.Instance.NormalEnemyData;
                break;
            case DataHolder.DifficultyMode.Hard:
                enemyMovement.Data = GPCtrl.Instance.HardEnemyData;
                break;
        }

        //Depending on doSpawnOnlyOnePerLine, set the "_enemies" list
        if (SpawnerData.spawningTimers[_currentSpawningTimerIndex].DoSpawnOnlyOnePerLine)
        {
            _enemies[SpawnPointIndex] = enemy;
            EnemyList.Add(enemyMovement);
        }
        else
        {
            _enemies.Add(enemy);
            EnemyList.Add(enemyMovement);
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
            Gizmos.DrawLine(enemySpawnPoint.position, enemySpawnPoint.position+Vector3.up*10000);
        }
        float height = SpawnerData.EnemyPrefab.GetComponent<EnemyMovement>().Data.FinalSpeedHeight;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, height, transform.position.z), new Vector3(1000,0,1000));
    }
}
