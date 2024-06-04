using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EnemyWeakSpotManagement : MonoBehaviour
{
    public EnemyMovement EnemyMovement;
    public List<TargetableSpot> WeakSpotSpawnList;
    [ReadOnly]
    public List<TargetableSpot> WeakSpotList;
    public int WeakSpotNum;
    public EnemyVFXData VFXData;

    void Start()
    {
        foreach (TargetableSpot weakSpot in WeakSpotSpawnList)
        {
            weakSpot.gameObject.SetActive(false);
        }
        for (int i = 0; i < WeakSpotNum; ++i)
        {
            TargetableSpot weakSpot = WeakSpotSpawnList[Random.Range(0, WeakSpotSpawnList.Count)];
            WeakSpotSpawnList.Remove(weakSpot);
            weakSpot.gameObject.SetActive(true);
            WeakSpotList.Add(weakSpot);
            GPCtrl.Instance.TargetableSpotList.Add(weakSpot);
            weakSpot.Enemy = this;
        }
    }

    public void Damage(TargetableSpot weakSpot)
    {
        WeakSpotList.Remove(weakSpot);
        if (WeakSpotList.Count == 0)
        {
            Death();
        }
    }

    public void Death()
    {
        GPCtrl.Instance.AddKilledEnemy();
        GPCtrl.Instance.EnemySpawner.EnemyList.Remove(EnemyMovement);
        Destroy(gameObject);
    }
}
