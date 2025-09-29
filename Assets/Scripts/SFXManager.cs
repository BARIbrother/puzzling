using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SFXManager : MonoBehaviour
{


    public static SFXManager Instance { get; private set; }

    public AudioSource audioSource;
    public AudioClip[] sfxClips;   // Inspector에서 여러 개 등록

    private Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>();

    //setting
    public AudioMixer audioMixer;
    public Slider volumeSlider;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        foreach (var clip in sfxClips)
        {
            sfxDict[clip.name] = clip;
        }
    }

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("Volume", 0f);
        volumeSlider.value = savedVolume;
        audioMixer.SetFloat("Volume", savedVolume);

        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void Play(string name, float volume = 1f, bool isLoop = false)
    {
        audioSource.volume = volume;
        audioSource.loop = isLoop;
        if (sfxDict.ContainsKey(name))
        {
            audioSource.clip = sfxDict[name];
            audioSource.Play();
        }
        else if (String.Equals(name, "endsound"))
        {
            audioSource.Stop();
        }
        else
        {
            Debug.Log("unavailable audio name");
        }
    }
    
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
        PlayerPrefs.SetFloat("Volume", volume);
    }
}
