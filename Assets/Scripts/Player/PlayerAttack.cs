using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Player Player;
    private SpringJoint _springJoint;
    [HideInInspector]
    public bool IsGrappling = false;
    private WeakSpot _currentWeakSpot;

    public void Attack()
    {
        if (IsGrappling) return;
        if (GPCtrl.Instance.WeakSpotList.Count == 0) return;
        float distance = 1000;
        WeakSpot weakSpot = null;
        for (int i = 0; i < GPCtrl.Instance.WeakSpotList.Count; i++)
        {
            float currentDistance = Vector3.Distance(transform.position, GPCtrl.Instance.WeakSpotList[i].transform.position);
            if (currentDistance < distance)
            {
                weakSpot = GPCtrl.Instance.WeakSpotList[i];
                distance = currentDistance;
            }
        }
        if (weakSpot == null || distance > Player.Data.attackDistance) return;
        Vector3 direction = weakSpot.transform.position - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, Player.Data.attackDistance))
        {
            GrappleWeakSpot(weakSpot);
        }       
    }

    public void GrappleWeakSpot(WeakSpot weakSpot)
    {
        IsGrappling = true;
        _currentWeakSpot = weakSpot;
        Vector3 direction = weakSpot.transform.position - transform.position;
        _springJoint = weakSpot.gameObject.AddComponent<SpringJoint>();
        _springJoint.autoConfigureConnectedAnchor = false;
        _springJoint.connectedAnchor = weakSpot.transform.position;
        _springJoint.connectedBody = Player.Rigibody;
        _springJoint.enableCollision = true;
        float distanceFromPoint = Vector3.Distance(transform.position, weakSpot.transform.position) + 10;

        _springJoint.maxDistance = distanceFromPoint * 0.8f;
        _springJoint.minDistance = distanceFromPoint * 0.25f;

        _springJoint.spring = 10;
        _springJoint.damper = 5f;
        _springJoint.massScale = 4.5f;


        Player.PlayerSwingingLeft.SwingLineRenderer.positionCount = 2;
        Player.PlayerSwingingLeft.SwingLineRenderer.SetPosition(1, weakSpot.transform.position); //to shoot from the hand of the player
        Player.PlayerSwingingRight.SwingLineRenderer.positionCount = 2;
        Player.PlayerSwingingRight.SwingLineRenderer.SetPosition(1, weakSpot.transform.position); //to shoot from the hand of the player

    }

    private void Update()
    {
        if (IsGrappling && _springJoint != null)
        {
            if (_springJoint.maxDistance > 0.5f)
            {
                _springJoint.maxDistance -= 0.1f;
            }
            else
            {
                Destroy(_springJoint);
                IsGrappling = false;
            }
        }
    }

    private void LateUpdate()
    {
        if (_springJoint) //Visual effect for swing line
        {
            Player.PlayerSwingingLeft.SwingLineRenderer.SetPosition(0, transform.position);
            if (Player.PlayerSwingingLeft.SwingLineRenderer.GetPosition(1) != _currentWeakSpot.transform.position)
            {
                Player.PlayerSwingingLeft.SwingLineRenderer.SetPosition(1, Vector3.Lerp(Player.PlayerSwingingLeft.SwingLineRenderer.GetPosition(1), _currentWeakSpot.transform.position, 0.1f));
            }
            Player.PlayerSwingingRight.SwingLineRenderer.SetPosition(0, transform.position);
            if (Player.PlayerSwingingRight.SwingLineRenderer.GetPosition(1) != _currentWeakSpot.transform.position)
            {
                Player.PlayerSwingingRight.SwingLineRenderer.SetPosition(1, Vector3.Lerp(Player.PlayerSwingingRight.SwingLineRenderer.GetPosition(1), _currentWeakSpot.transform.position, 0.1f));
            }
        }
    }
}
