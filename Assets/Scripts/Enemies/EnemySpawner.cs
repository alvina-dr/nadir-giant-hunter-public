using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [TitleGroup("Components")]
    public EnemySpawnerData spawnerData;
    [SerializeField]
    private Transform _enemiesParent;
    [SerializeField]
    private bool doSpawnOnlyOnePerLine;

    [SerializeField]
    private List<Transform> EnemiesSpawnPoints = new List<Transform>();
    [SerializeField, Sirenix.OdinInspector.ReadOnly]
    private List<GameObject> Enemies = new List<GameObject>();

    

    // Start is called before the first frame update
    void Start()
    {
        if (doSpawnOnlyOnePerLine)
        {
            foreach (var item in EnemiesSpawnPoints)
            {
                Enemies.Add(null);
            }
        }
        StartCoroutine(SpawnCycle());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnCycle()
    {
        while (true) {
            SpawnNewEnemy();
            yield return new WaitForSeconds(spawnerData.SpawnCycle);
        }
    }

    public void SpawnNewEnemy()
    {
        int SpawnPointIndex = Random.Range(0, EnemiesSpawnPoints.Count);
        if((!doSpawnOnlyOnePerLine && Enemies.Count >= spawnerData.MaxEnemies) || (doSpawnOnlyOnePerLine && Enemies[SpawnPointIndex] != null))
        {
            return;
        }
        GameObject enemy = Instantiate(spawnerData.EnemyPrefab);
        Enemies.Add(enemy);
        enemy.transform.parent = _enemiesParent;

        Transform pos = EnemiesSpawnPoints[SpawnPointIndex];
        enemy.transform.position = pos.position;
        enemy.transform.rotation = pos.rotation;
        if (doSpawnOnlyOnePerLine)
        {
            Enemies[SpawnPointIndex] = enemy;
        }
        else
        {
            Enemies.Add(enemy);
        }
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3[] points = new Vector3[20];

        for (int i = 0; i < 20; i++)
        {
            float angle = i * (360/20);
            points[i] = transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle),0,Mathf.Sin(Mathf.Deg2Rad * angle))*spawnerData.SpawnCircleWidth;
        }

        Gizmos.DrawLineStrip(points, false);
    }
}
