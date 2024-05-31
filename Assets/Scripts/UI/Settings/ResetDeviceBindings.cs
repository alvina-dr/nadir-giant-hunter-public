using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResetDeviceBindings : MonoBehaviour
{
    [SerializeField] private InputActionAsset _gameplayInputActions;
    [SerializeField] private InputActionAsset _cameraInputActions;
    [SerializeField] private string _targetControlScheme;

    public void ResetAllBindings()
    {
        foreach (InputActionMap map in _gameplayInputActions.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }
        foreach (InputActionMap map in _cameraInputActions.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }
    }

    public void ResetControlSchemeBinding()
    {
        foreach (InputActionMap map in _gameplayInputActions.actionMaps)
        {
            foreach (InputAction action in map.actions)
            {
                action.RemoveBindingOverride(InputBinding.MaskByGroup(_targetControlScheme));
            }
        }
        foreach (InputActionMap map in _cameraInputActions.actionMaps)
        {
            foreach (InputAction action in map.actions)
            {
                action.RemoveBindingOverride(InputBinding.MaskByGroup(_targetControlScheme));
            }
        }
    }
}
