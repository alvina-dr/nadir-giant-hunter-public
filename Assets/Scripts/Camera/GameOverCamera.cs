using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemies;

public class GameOverCamera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;

    public void FocusEnemy(EnemyMovement enemy)
    {
        _virtualCamera.LookAt = enemy.transform;
        _virtualCamera.Follow = enemy.transform;
        _virtualCamera.enabled = true;
    }
}
