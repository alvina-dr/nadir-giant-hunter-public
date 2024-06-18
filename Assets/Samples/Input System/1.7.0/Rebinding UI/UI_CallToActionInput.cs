using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace UnityEngine.InputSystem.Samples.RebindUI
{
    public class UI_CallToActionInput : MonoBehaviour
    {
        [SerializeField] ControlerIcons _icons;
        [SerializeField] public Image _image;
        [SerializeField] public InputActionReference _action;

        private void Start()
        {
            InputSystem.onDeviceChange += SetImage;
            RebindActionUI.OnInputChanged += SetImage;
            SetImage(null, InputDeviceChange.Added);
        }

        public void SetImage(InputDevice inputDevice, InputDeviceChange inputDeviceChange)
        {
            SetImage();
        }

        public void SetImage()
        {
            if (_action == null) return;
            string inputName = string.Empty;
            if (Gamepad.all.Count > 0)
            {
                for (int i = 0; i < _action.action.bindings.Count; i++)
                {
                    if (_action.action.bindings[i].groups == "Gamepad") inputName = _action.action.bindings[i].effectivePath;
                }
                inputName = inputName.Split("/").Last();
                _image.sprite = _icons.xbox.GetSprite(inputName);

            }
            else
            {
                for (int i = 0; i < _action.action.bindings.Count; i++)
                {
                    if (_action.action.bindings[i].groups == "Keyboard") inputName = _action.action.bindings[i].effectivePath;
                }
                inputName = inputName.Split("/").Last();
                _image.sprite = _icons.computer.GetSprite(inputName);
            }
        }

        private void OnDestroy()
        {
            InputSystem.onDeviceChange -= SetImage;
            RebindActionUI.OnInputChanged -= SetImage;
        }
    }
}