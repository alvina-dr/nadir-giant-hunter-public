using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_InputIndication : MonoBehaviour
{
    [SerializeField] private Image _img;

    public void ShowIndicatorAt(Vector3 worldPosition)
    {
        float minX = _img.GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;

        float minY = _img.GetPixelAdjustedRect().height / 2;
        float maxY = Screen.height - minY;

        Vector2 pos = Camera.main.WorldToScreenPoint(worldPosition);

        pos.x = Mathf.Clamp(pos.x, minX, maxX); 
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.localScale = Vector3.one;
        transform.position = pos;
    }

    public void HideIndicator()
    {
        transform.localScale = Vector3.zero;
    }
}
