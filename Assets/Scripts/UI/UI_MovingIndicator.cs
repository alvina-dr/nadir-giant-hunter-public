using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_MovingIndicator : MonoBehaviour
{
    [SerializeField] private Image _img;
    enum IndicatorType
    {
        TargetableSpot = 0,
        WarningHighMonster = 1,
        SwingPoint = 2
    }
    [SerializeField] private IndicatorType IndicatorCurrentType;

    private void Awake()
    {
        _img.material = new Material(_img.material);
        switch(IndicatorCurrentType)
        {
            case IndicatorType.TargetableSpot:
                _img.material.SetFloat("_Warning_Monster_High_Visibility", 0f);
                _img.material.SetFloat("_Lock_Visibility", 1f);
                break;
            case IndicatorType.WarningHighMonster:
                _img.material.SetFloat("_Warning_Monster_High_Visibility", 1f);
                _img.material.SetFloat("_Lock_Visibility", 0f);
                break;
            case IndicatorType.SwingPoint:
                break;
        }
    }

    public void ShowIndicatorAt(Vector3 worldPosition)
    {
        float minX = _img.GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;

        float minY = _img.GetPixelAdjustedRect().height / 2;
        float maxY = Screen.height - minY;

        Vector2 pos = Camera.main.WorldToScreenPoint(worldPosition);

        if (Vector3.Dot(worldPosition - Camera.main.transform.position, Camera.main.transform.forward) < 0)
        {
            if (pos.x < Screen.width / 2)
            {
                pos.x = maxX;
            } else
            {
                pos.x = minX;
            }
        }

        pos.x = Mathf.Clamp(pos.x, minX, maxX); 
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.localScale = Vector3.one;
        transform.position = pos;
    }

    public void HideIndicator()
    {
        transform.localScale = Vector3.zero;
    }

    public void SetupAppearance(bool lockVisible, bool highMonsterVisible)
    {
        _img.material.SetFloat("_Lock_Visibility", lockVisible ? 1 : 0);
        _img.material.SetFloat("_Warning_Monster_High_Visibility", highMonsterVisible ? 1 : 0);
    }

    public void SetUnscaledTime()
    {
        _img.material.SetFloat("_unscaled_time", _img.material.GetFloat("_unscaled_time") + Time.unscaledDeltaTime); ;
    }
}