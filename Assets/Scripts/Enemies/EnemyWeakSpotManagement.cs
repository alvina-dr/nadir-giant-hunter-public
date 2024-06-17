using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Enemies;

public class EnemyWeakSpotManagement : MonoBehaviour
{
    public EnemyMovement EnemyMovement;
    public IKHarmAnimation IKHarmAnimation;
    public IKHarmWiggle IKHarmWiggle;
    public Waving Waving;
    public List<TargetableSpot> WeakSpotSpawnList;
    [ReadOnly]
    public List<TargetableSpot> WeakSpotList;
    public int WeakSpotNum;
    public EnemyVFXData VFXData;
    public Rigidbody rigidbody;
    public List<Tentacle> Tentacles = new List<Tentacle>();

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
        weakSpot.transform.parent = null;
        WeakSpotList.Remove(weakSpot);
        if (WeakSpotList.Count == 0)
        {
            Death();
        }
    }

    [Button]
    public void Death()
    {
        GPCtrl.Instance.AddKilledEnemy();
        GPCtrl.Instance.EnemySpawner.EnemyList.Remove(EnemyMovement);
        if (GPCtrl.Instance.Player.PlayerSwingingLeft.EndSwingLinePoint.parent != null)
            Debug.Log("end swing line point left : " + GPCtrl.Instance.Player.PlayerSwingingLeft.EndSwingLinePoint.parent.name);
        if (GPCtrl.Instance.Player.PlayerSwingingRight.EndSwingLinePoint.parent != null)
            Debug.Log("end swing line point right : " + GPCtrl.Instance.Player.PlayerSwingingRight.EndSwingLinePoint.parent.name);
        GPCtrl.Instance.Player.PlayerSwingingLeft.EndSwingLinePoint.parent = null;
        GPCtrl.Instance.Player.PlayerSwingingRight.EndSwingLinePoint.parent = null;
        EnemyMovement.SoundData.SFX_Giant_Roar_Death.Post(gameObject);
        ActivateRagDoll();
        StartCoroutine(DestroyAfter(10));
    }

    [Button]
    public void ActivateRagDoll()
    {
        rigidbody.isKinematic = false;
        EnemyMovement.enabled = false;
        IKHarmAnimation.enabled = false;
        IKHarmWiggle.enabled = false;
        Waving.enabled = false;
        foreach (Tentacle tentacle in Tentacles)
        {
            tentacle.ActivateRagdoll();
        }
    }

    public IEnumerator DestroyAfter(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
