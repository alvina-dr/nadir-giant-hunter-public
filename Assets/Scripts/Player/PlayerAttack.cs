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
            if (hit.transform.gameObject != weakSpot.gameObject) return;
            GrappleWeakSpot(weakSpot);
        }       
    }

    public void GrappleWeakSpot(WeakSpot weakSpot)
    {
        IsGrappling = true;
        _currentWeakSpot = weakSpot;
        Player.Rigibody.useGravity = false;
        Player.Rigibody.velocity = Vector3.zero;
        _springJoint = gameObject.AddComponent<SpringJoint>();
        _springJoint.autoConfigureConnectedAnchor = false;
        _springJoint.connectedAnchor = Vector3.zero;
        _springJoint.connectedBody = weakSpot.Rigidbody;

        _springJoint.spring = 20;
        _springJoint.damper = 0f;
        _springJoint.massScale = Player.Data.dragForce;

        Player.PlayerSwingingLeft.SwingLineRenderer.positionCount = 2;
        Player.PlayerSwingingLeft.SwingLineRenderer.SetPosition(1, weakSpot.transform.position); //to shoot from the hand of the player
        Player.PlayerSwingingRight.SwingLineRenderer.positionCount = 2;
        Player.PlayerSwingingRight.SwingLineRenderer.SetPosition(1, weakSpot.transform.position); //to shoot from the hand of the player
    }

    private void Update()
    {
        if (IsGrappling && _springJoint != null)
        {
            if (Vector3.Distance(transform.position, _currentWeakSpot.transform.position) < 2f)
            {
                Destroy(_springJoint);
                _springJoint = null;
                IsGrappling = false;
                Player.PlayerSwingingLeft.SwingLineRenderer.positionCount = 0;
                Player.PlayerSwingingRight.SwingLineRenderer.positionCount = 0;
                Player.Rigibody.useGravity = true;
            }
        }
    }

    private void LateUpdate()
    {
        if (_springJoint && IsGrappling) //Visual effect for swing line
        {
            if (Player.PlayerSwingingLeft.SwingLineRenderer.positionCount == 2)
            {
                Player.PlayerSwingingLeft.SwingLineRenderer.SetPosition(0, Player.PlayerSwingingLeft.StartSwingLinePoint.position);
                if (Player.PlayerSwingingLeft.SwingLineRenderer.GetPosition(1) != _currentWeakSpot.transform.position)
                {
                    Player.PlayerSwingingLeft.SwingLineRenderer.SetPosition(1, Vector3.Lerp(Player.PlayerSwingingLeft.SwingLineRenderer.GetPosition(1), _currentWeakSpot.transform.position, 0.1f));
                }
            }
            if (Player.PlayerSwingingRight.SwingLineRenderer.positionCount == 2)
            {
                Player.PlayerSwingingRight.SwingLineRenderer.SetPosition(0, Player.PlayerSwingingRight.StartSwingLinePoint.position);
                if (Player.PlayerSwingingRight.SwingLineRenderer.GetPosition(1) != _currentWeakSpot.transform.position)
                {
                    Player.PlayerSwingingRight.SwingLineRenderer.SetPosition(1, Vector3.Lerp(Player.PlayerSwingingRight.SwingLineRenderer.GetPosition(1), _currentWeakSpot.transform.position, 0.1f));
                }
            }
        }
    }
}
