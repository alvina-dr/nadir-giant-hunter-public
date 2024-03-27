using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GPCtrl : MonoBehaviour
{
    #region Singleton
    public static GPCtrl Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Debug.LogError("Ahaha");
        WeakSpotList = FindObjectsByType<WeakSpot>(FindObjectsSortMode.InstanceID).ToList();
    }
    #endregion

    public Player Player;
    public CameraThirdPerson CameraThirdPerson;
    public List<WeakSpot> WeakSpotList;
}
