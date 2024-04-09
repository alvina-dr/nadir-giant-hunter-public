using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UI_Button : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public void OnDeselect(BaseEventData eventData)
    {
        transform.DOScale(1f, .3f);
    }

    public void OnSelect(BaseEventData eventData)
    {
        transform.DOScale(1.1f, .3f);        
    }
}
