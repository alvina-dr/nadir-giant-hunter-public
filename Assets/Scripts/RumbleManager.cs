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
        if (!PlayerPrefs.HasKey("Vibration")) PlayerPrefs.SetInt("Vibration", 1);

        pad = Gamepad.current;
        if (pad != null)
        {
            if (PlayerPrefs.GetInt("Vibration") == 0)
            {
                pad.SetMotorSpeeds(0, 0);
                return;
            }

            pad.SetMotorSpeeds(lowFrequency, highFrequency);
            DOVirtual.DelayedCall(duration, () =>
            {
                pad.SetMotorSpeeds(0, 0);
            });
        }
    }

    public void StartPulse(float lowFrequency, float highFrequency)
    {
        if (!PlayerPrefs.HasKey("Vibration")) PlayerPrefs.SetInt("Vibration", 1);
        
        pad = Gamepad.current;
        if (pad != null)
        {
            if (PlayerPrefs.GetInt("Vibration") == 0)
            {
                pad.SetMotorSpeeds(0, 0);
                return;
            }

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
