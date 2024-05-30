using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class Bezier {
	private class LengthData
	{
		public float Length;
		public float Delta;
		public LengthData(float length, float delta)
		{
			Length = length;
			Delta = delta;
		}
	}
	private Vector3[] _points;
	private List<LengthData> _lengths = new List<LengthData>();
	public float TotalLength;

	public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
		t = Mathf.Clamp01(t);
		float oneMinusT = 1f - t;
		return
			oneMinusT * oneMinusT * p0 +
			2f * oneMinusT * t * p1 +
			t * t * p2;
	}

	public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
		return
			2f * (1f - t) * (p1 - p0) +
			2f * t * (p2 - p1);
	}

	public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
		t = Mathf.Clamp01(t);
		float OneMinusT = 1f - t;
		return
			OneMinusT * OneMinusT * OneMinusT * p0 +
			3f * OneMinusT * OneMinusT * t * p1 +
			3f * OneMinusT * t * t * p2 +
			t * t * t * p3;
	}

	public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
		t = Mathf.Clamp01(t);
		float oneMinusT = 1f - t;
		return
			3f * oneMinusT * oneMinusT * (p1 - p0) +
			6f * oneMinusT * t * (p2 - p1) +
			3f * t * t * (p3 - p2);
	}

	public static float GetLength(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int precision)
	{
		float length = 0;
		for (int i = 0; i < precision; i++)
		{
			float delta = (float)i / precision;
			float nextDelta = (float)(i + 1) / precision;
			Vector3 pos1 = GetPoint(p0, p1, p2, p3, delta);
			Vector3 pos2 = GetPoint(p0, p1, p2, p3, nextDelta);

			length += Vector3.Distance(pos1, pos2);

		}
		return length;
	}

	//with array
	public static Vector3 GetPoint(Vector3[] points, float t)
	{
		return GetPoint(points[0], points[1], points[2], points[3], t);
	}

	public static Vector3 GetFirstDerivative(Vector3[] points, float t)
	{
		return GetFirstDerivative(points[0], points[1], points[2], points[3], t);
	}

	public static float GetLength(Vector3[] points, int precision)
	{
		return GetLength(points[0], points[1], points[2], points[3], precision);
	}

    public Bezier(Vector3[] points, int precision)
	{
		_points = points;
		SampleLengths(precision);

    }

	private void SampleLengths(int precision)
	{
        float length = 0;
		Vector3 lastPoint = _points[0];
        for (int i = 0; i < precision; i++)
        {
            float delta = (float)i / precision;
            Vector3 pos1 = GetPoint(_points[0], _points[1], _points[2], _points[3], delta);
            length += Vector3.Distance(pos1, lastPoint);
            lastPoint = pos1;
			LengthData lengthData = new LengthData(length, delta);
            _lengths.Add(lengthData);
			if (i == precision - 1)
			{
                TotalLength = length;
            }
        }
    }

	public Vector3 GetPoint(float t)
	{
		float tofind = t * TotalLength;
		foreach (LengthData length in _lengths)
		{
			if (length.Length >= tofind)
				return GetPoint(_points, length.Delta);
		}
        return GetPoint(_points, 1);
    }
}