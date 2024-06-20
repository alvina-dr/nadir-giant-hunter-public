using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UI_BlinkText : MonoBehaviour
{
    void Start()
    {
        transform.DOScale(1.15f, .3f).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
    }
}
