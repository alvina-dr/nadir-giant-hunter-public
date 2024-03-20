using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extDebug.Menu;

public class DebugMenu : MonoBehaviour
{
    Color _color;
    private void Awake()
    {
        SetupDebugMenu();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (DM.IsVisible) DM.Back();
            else DM.Open();
        }
    }

    public void SetupDebugMenu()
    {
        DM.Root.Clear();
        DM.Add("Print/HelloWorld", action => Debug.Log("Hello World"));
        DM.Add("Grapple/StartCurveBoost", () => GPCtrl.Instance.player.data.startCurveBoost, v => GPCtrl.Instance.player.data.startCurveBoost = v);
        DM.Add("Grapple/EndCurveBoost", () => GPCtrl.Instance.player.data.endCurveBoost, v => GPCtrl.Instance.player.data.endCurveBoost = v);
        //DM.Add("Values/Color", () => _color, v => _color = v, order: 18).SetPrecision(2);
    }
}
