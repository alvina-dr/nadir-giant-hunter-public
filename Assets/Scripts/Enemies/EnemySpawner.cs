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


    [SerializeField, Sirenix.OdinInspector.ReadOnly]
    private List<GameObject> Enemies = new List<GameObject>();

    

    // Start is called before the first frame update
    void Start()
    {
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
        if (Enemies.Count >= spawnerData.MaxEnemies)
        {
            return;
        }
        GameObject enemy = Instantiate(spawnerData.EnemyPrefab);
        Enemies.Add(enemy);
        enemy.transform.parent = _enemiesParent;
        Vector3 pos = Random.insideUnitSphere.normalized * spawnerData.SpawnCircleWidth;
        pos = transform.position + new Vector3(pos.x, 0, pos.z);
        enemy.transform.position = pos;
        enemy.transform.rotation = Quaternion.LookRotation((enemy.transform.position - transform.position).normalized);
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
