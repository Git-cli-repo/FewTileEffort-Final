using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;
using System.Collections.Generic;
using System.Linq;

public class ManageSettingsScreen : MonoBehaviour
{
    public Slider sfxVolume;
    public Slider musicVolume;
    public Slider playerRed;
    public Slider playerGreen;
    public Slider playerBlue;
    public AudioMixer mainMixer;
    public GameObject colorDisplay;
    public void Start()
    {
        mainMixer.SetFloat("SoundEffectsVolume", FloatToDecibel(AchievementManager.Instance.ReadFromFile("tracker.json", "sfxVolume", 0f)));
        mainMixer.SetFloat("MusicVolume", FloatToDecibel(AchievementManager.Instance.ReadFromFile("tracker.json", "musicVolume", 0f)));
        mainMixer.GetFloat("SoundEffectsVolume", out float sfx);
        mainMixer.GetFloat("MusicVolume", out float mus);
        sfxVolume.SetValueWithoutNotify(DecibelToFloat(sfx));
        musicVolume.SetValueWithoutNotify(DecibelToFloat(mus));
    }

    public void EditSFXVolume()
    {
        float sliderVal = sfxVolume.value;
        float dB = FloatToDecibel(sliderVal);
        Debug.LogWarning($"SFX slider:{sliderVal} -> dB:{dB}");

        mainMixer.SetFloat("SoundEffectsVolume", dB);
        mainMixer.GetFloat("SoundEffectsVolume", out float b);
        Debug.LogWarning($"SFX readback: {b}");
    }

    public void EditMusicVolume()
    {
        mainMixer.SetFloat("MusicVolume", FloatToDecibel(musicVolume.value));
        AchievementManager.Instance.WriteToFile("tracker.json", musicVolume.value, "musicVolume");
        Debug.Log(mainMixer.GetFloat("MusicVolume", out float b));
        Debug.LogWarning(b);
    }

    public void EditPlayerColor()
    {
        Image panel = colorDisplay.GetComponent<Image>();
        panel.color = new Color(playerRed.value, playerGreen.value, playerBlue.value, 1f);
        AchievementManager.Instance.WriteToFile("tracker.json", $"{playerRed.value},{playerGreen.value},{playerBlue.value}", "playerColor");
        Debug.LogWarning(AchievementManager.Instance.ReadFromFile("tracker.json", "playerColor", false));
    }

    public float FloatToDecibel(float input)
    {
        float g = (float)(20 * Math.Log10(input));
        if (g > 0)
        {
            g = 0;
        }
        else if (g < -80)
        {
            g = -80;
        }
        return g;
    }

    public float DecibelToFloat(float input)
    {
        float g = (float)Math.Pow(10, input / 20);
        if (g > 1)
        {
            g = 1;
        }
        else if (g < 0)
        {
            g = 0;
        }

        return g;
    }

    public Color ParseStringToColor(string hexCode)
    {
        List<float> rgb = new List<float>();
        for (int i = 0; i < hexCode.Split(",").ToList().Count; i++)
        {
            rgb.Add(float.Parse(hexCode.Split(",").ToList()[i]));
        }
        Color newColor = new Color(rgb[0], rgb[1], rgb[2], 1f);
        return newColor;
    }
}