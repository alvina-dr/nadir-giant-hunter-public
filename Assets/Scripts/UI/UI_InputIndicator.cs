using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_InputIndicator : MonoBehaviour
{
    [SerializeField] Image _image;
    public void SetVisible(bool visible)
    {
        byte alpha = 255;
        if (!visible) alpha = 0;
        _image.color = new Color32(255, 255, 255, alpha);
    }
}
