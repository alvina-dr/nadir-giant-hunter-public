using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputSystem.Samples.RebindUI
{
    [CreateAssetMenu(fileName = "ControlerIcons", menuName = "ScriptableObject/ControlerIcons", order = 1)]
    public class ControlerIcons : ScriptableObject
    {
        public GamepadIcons xbox;
        public GamepadIcons ps4;
        public ComputerIcons computer;

        [Serializable]
        public struct GamepadIcons
        {
            public Sprite buttonSouth;
            public Sprite buttonNorth;
            public Sprite buttonEast;
            public Sprite buttonWest;
            public Sprite startButton;
            public Sprite selectButton;
            public Sprite leftTrigger;
            public Sprite rightTrigger;
            public Sprite leftShoulder;
            public Sprite rightShoulder;
            public Sprite dpad;
            public Sprite dpadUp;
            public Sprite dpadDown;
            public Sprite dpadLeft;
            public Sprite dpadRight;
            public Sprite leftStick;
            public Sprite rightStick;
            public Sprite leftStickPress;
            public Sprite rightStickPress;

            public Sprite GetSprite(string controlPath)
            {
                // From the input system, we get the path of the control on device. So we can just
                // map from that to the sprites we have for gamepads.
                switch (controlPath)
                {
                    case "buttonSouth": return buttonSouth;
                    case "buttonNorth": return buttonNorth;
                    case "buttonEast": return buttonEast;
                    case "buttonWest": return buttonWest;
                    case "start": return startButton;
                    case "select": return selectButton;
                    case "leftTrigger": return leftTrigger;
                    case "rightTrigger": return rightTrigger;
                    case "leftShoulder": return leftShoulder;
                    case "rightShoulder": return rightShoulder;
                    case "dpad": return dpad;
                    case "dpad/up": return dpadUp;
                    case "dpad/down": return dpadDown;
                    case "dpad/left": return dpadLeft;
                    case "dpad/right": return dpadRight;
                    case "leftStick": return leftStick;
                    case "rightStick": return rightStick;
                    case "leftStickPress": return leftStickPress;
                    case "rightStickPress": return rightStickPress;
                }
                return null;
            }
        }


        [Serializable]
        public struct ComputerIcons
        {
            public Sprite leftClick;
            public Sprite rightClick;
            public Sprite middleClick;
            public Sprite aKey;
            public Sprite bKey;
            public Sprite cKey;
            public Sprite dKey;
            public Sprite eKey;
            public Sprite fKey;
            public Sprite gKey;
            public Sprite hKey;
            public Sprite iKey;
            public Sprite jKey;
            public Sprite kKey;
            public Sprite lKey;
            public Sprite mKey;
            public Sprite nKey;
            public Sprite oKey;
            public Sprite pKey;
            public Sprite qKey;
            public Sprite rKey;
            public Sprite sKey;
            public Sprite tKey;
            public Sprite uKey;
            public Sprite vKey;
            public Sprite wKey;
            public Sprite xKey;
            public Sprite yKey;
            public Sprite zKey;
            public Sprite escapeKey;
            public Sprite spaceKey;
            public Sprite upArrowKey;
            public Sprite downArrowKey;
            public Sprite leftArrowKey;
            public Sprite rightArrowKey;

            public Sprite GetSprite(string controlPath)
            {
                switch (controlPath)
                {
                    case "leftButton": return leftClick;
                    case "rightButton": return rightClick;
                    case "middleButton": return middleClick;
                    case "a": return aKey;
                    case "b": return bKey;
                    case "c": return cKey;
                    case "d": return dKey;
                    case "e": return eKey;
                    case "f": return fKey;
                    case "g": return gKey;
                    case "h": return hKey;
                    case "i": return iKey;
                    case "j": return jKey;
                    case "k": return kKey;
                    case "l": return lKey;
                    case "m": return mKey;
                    case "n": return nKey;
                    case "o": return oKey;
                    case "p": return pKey;
                    case "q": return qKey;
                    case "r": return rKey;
                    case "s": return sKey;
                    case "t": return tKey;
                    case "u": return uKey;
                    case "v": return vKey;
                    case "w": return wKey;
                    case "x": return xKey;
                    case "y": return yKey;
                    case "z": return zKey;
                    case "escape": return escapeKey;
                    case "space": return spaceKey;
                    case "upArrow": return upArrowKey;
                    case "downArrow": return downArrowKey;
                    case "leftArrow": return leftArrowKey;
                    case "rightArrow": return rightArrowKey;
                }
                return null;
            }
        }
    }
}

