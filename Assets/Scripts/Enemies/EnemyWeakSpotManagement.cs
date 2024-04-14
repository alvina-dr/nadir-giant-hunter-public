using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EnemyWeakSpotManagement : MonoBehaviour
{
    public List<WeakSpot> WeakSpotSpawnList;
    [ReadOnly]
    public List<WeakSpot> WeakSpotList;
    public int WeakSpotNum;
    void Start()
    {
        foreach (WeakSpot weakSpot in WeakSpotSpawnList)
        {
            weakSpot.gameObject.SetActive(false);
        }
        for (int i = 0; i < WeakSpotNum; ++i)
        {
            WeakSpot weakSpot = WeakSpotSpawnList[Random.Range(0, WeakSpotSpawnList.Count)];
            WeakSpotSpawnList.Remove(weakSpot);
            weakSpot.gameObject.SetActive(true);
            WeakSpotList.Add(weakSpot);
            GPCtrl.Instance.WeakSpotList.Add(weakSpot);
            weakSpot.Enemy = this;
        }
    }

    public void Damage(WeakSpot weakSpot)
    {
        WeakSpotList.Remove(weakSpot);
        if (WeakSpotList.Count == 0)
        {
            Death();
        }
    }

    public void Death()
    {
        Destroy(gameObject);
    }
}
