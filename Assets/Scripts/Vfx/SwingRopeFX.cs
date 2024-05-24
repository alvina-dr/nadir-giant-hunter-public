using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingRopeFX : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    private Spring _spring;
    private Vector3 _currentGrapplePosition;
    public int _quality;
    public float _damper;
    public float _strength;
    public float _velocity;
    public float _waveCount;
    public float _waveHeight;
    public AnimationCurve _affectCurve;

    private void Awake()
    {
        _spring = new Spring();
        _spring.SetTarget(0);
    }

    public void DrawRope(PlayerSwinging playerSwinging)
    {
        //if not grappling don't draw rope
        if (!playerSwinging.IsSwinging)
        {
            _currentGrapplePosition = playerSwinging.StartSwingLinePoint.position;
            _spring.Reset();

            if (_lineRenderer.positionCount > 0)
                _lineRenderer.positionCount = 0;
            return;
        }

        if (_lineRenderer.positionCount == 0)
        {
            _spring.SetVelocity(_velocity);
            _lineRenderer.positionCount = _quality + 1;
        }

        _spring.SetDamper(_damper);
        _spring.SetStrength(_strength);
        _spring.Update(Time.deltaTime);

        Vector3 grapplePoint = playerSwinging.EndSwingLinePoint.position;
        Vector3 gunTipPosition = playerSwinging.StartSwingLinePoint.position;
        var up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up;

        _currentGrapplePosition = Vector3.Lerp(_currentGrapplePosition, grapplePoint, Time.deltaTime * 12f);

        for (var i = 0; i < _quality + 1; i++)
        {
            var delta = i / (float)_quality;
            var offset = up * _waveHeight * Mathf.Sin(delta * _waveCount * Mathf.PI) * _spring.Value *
                         _affectCurve.Evaluate(delta);

            _lineRenderer.SetPosition(i, Vector3.Lerp(gunTipPosition, _currentGrapplePosition, delta) + offset);
        }
    }
}
