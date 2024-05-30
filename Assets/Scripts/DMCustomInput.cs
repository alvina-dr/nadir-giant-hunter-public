using extDebug.Menu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMCustomInput : IDMInput
{
    public EventKey GetKey(bool isVisible, out bool shift)
    {
        shift = false;

        if (Input.GetKey(KeyCode.Y))
            return EventKey.ToggleMenu;

        if (isVisible)
        {
            shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            if (Input.GetKey(KeyCode.UpArrow))
                return EventKey.Up;
            if (Input.GetKey(KeyCode.DownArrow))
                return EventKey.Down;
            if (Input.GetKey(KeyCode.LeftArrow))
                return EventKey.Left;
            if (Input.GetKey(KeyCode.RightArrow))
                return EventKey.Right;
            if (Input.GetKey(KeyCode.Backspace))
                return EventKey.Back;
            if (Input.GetKey(KeyCode.R))
                return EventKey.Reset;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKey(KeyCode.LeftArrow))
                return EventKey.Left;
            if (Input.GetKey(KeyCode.RightArrow))
                return EventKey.Right;
            if (Input.GetKey(KeyCode.R))
                return EventKey.Reset;
        }

        return EventKey.None;
    }
}