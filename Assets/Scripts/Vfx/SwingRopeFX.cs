using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingRopeFX : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private LineRenderer _highlightLineRenderer;
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

    public void DrawRope(Vector3 startPoint, Vector3 endPoint)
    {
        if (_lineRenderer.positionCount == 0)
        {
            _spring.SetVelocity(_velocity);
            _lineRenderer.positionCount = _quality + 1;
            _highlightLineRenderer.positionCount = _quality + 1;
        }

        _spring.SetDamper(_damper);
        _spring.SetStrength(_strength);
        _spring.Update(Time.deltaTime);

        Vector3 grapplePoint = endPoint;
        Vector3 gunTipPosition = startPoint;
        var up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up;

        _currentGrapplePosition = Vector3.Lerp(_currentGrapplePosition, grapplePoint, Time.deltaTime * 12f);

        for (var i = 0; i < _quality + 1; i++)
        {
            var delta = i / (float)_quality;
            var offset = up * _waveHeight * Mathf.Sin(delta * _waveCount * Mathf.PI) * _spring.Value *
                         _affectCurve.Evaluate(delta);

            _lineRenderer.SetPosition(i, Vector3.Lerp(gunTipPosition, _currentGrapplePosition, delta) + offset);
            _highlightLineRenderer.SetPosition(i, Vector3.Lerp(gunTipPosition, _currentGrapplePosition, delta) + offset + new Vector3(.02f, 0, 0));
        }
    }

    public void HideRope(Vector3 startPoint)
    {
        _currentGrapplePosition = startPoint;
        _spring.Reset();

        if (_lineRenderer.positionCount > 0)
        {
            _lineRenderer.positionCount = 0;
            _highlightLineRenderer.positionCount = 0;
        }
    }
}
