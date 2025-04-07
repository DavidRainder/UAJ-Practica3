using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public void ToggleFullScreen(bool fullscreen)
    {
        GameManager.Instance.ToggleFullScreen(fullscreen);
    }

    public void ChangeVFXSoundValue(int sound)
    {
        // Diego haz tus cosas
    }

    public void ChangeMusicValue(int sound)
    {
        // Diego haz tus cosas
    }
}
