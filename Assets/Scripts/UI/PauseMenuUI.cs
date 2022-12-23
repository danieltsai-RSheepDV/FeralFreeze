using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    private Bus masterBus;
    [SerializeField] private Slider volume;
    
    private void Start()
    {
        gameObject.SetActive(false);
        
        float vol = PlayerPrefs.HasKey("volume") ? PlayerPrefs.GetFloat("volume") : 10;
        
        masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
        masterBus.setVolume(vol);
        volume.value = vol;
    }

    public void onVolChanged(float vol)
    {
        masterBus.setVolume(vol);
        PlayerPrefs.SetFloat("volume", vol);
    }

    public void Resume()
    {
        gameObject.SetActive(false);
        GameManager.Paused = false;
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
