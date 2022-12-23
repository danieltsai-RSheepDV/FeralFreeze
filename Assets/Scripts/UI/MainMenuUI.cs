using System;
using System.Runtime.InteropServices;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    
    private MusicInstance music;

    private Bus masterBus;
    [SerializeField] private Slider volume;

    private void Start()
    {
        music = new MusicInstance("event:/startGame");
        music.start();
        
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

    public void Quit()
    {
        Application.Quit();
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("GameScene");
        music.Destroy();
    }
}
