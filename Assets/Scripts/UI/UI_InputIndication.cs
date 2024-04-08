using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_InputIndication : MonoBehaviour
{
    public void ShowIndicatorAt(Vector3 worldPosition)
    {
        transform.localScale = Vector3.one;
        transform.position = Camera.main.WorldToScreenPoint(worldPosition);
    }

    public void HideIndicator()
    {
        transform.localScale = Vector3.zero;
    }
}
