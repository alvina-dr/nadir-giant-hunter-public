using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class RumbleManager : MonoBehaviour
{
    private Gamepad pad;

    public void PulseFor(float lowFrequency, float highFrequency, float duration)
    {
        pad = Gamepad.current;

        if (pad != null)
        {
            pad.SetMotorSpeeds(lowFrequency, highFrequency);
            DOVirtual.DelayedCall(duration, () =>
            {
                pad.SetMotorSpeeds(0, 0);
            });
        }
    }

    public void StartPulse(float lowFrequency, float highFrequency)
    {
        pad = Gamepad.current;

        if (pad != null)
        {
            pad.SetMotorSpeeds(lowFrequency, highFrequency);
        }
    }

    public void StopPulse()
    {
        pad = Gamepad.current;

        if (pad != null)
        {
            pad.SetMotorSpeeds(0f, 0f);
        }
    }
}