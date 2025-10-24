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

    public ScrollRect logRect;
    public Slider sensitivitySlider;

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
        float savedVolume = SaverLoader.LoadBgm();
        volumeSlider.value = savedVolume;
        audioMixer.SetFloat("Volume", savedVolume);

        volumeSlider.onValueChanged.AddListener(SetVolume);

        float savedValue = SaverLoader.LoadSensitivity();
        sensitivitySlider.value = savedValue;
        logRect.scrollSensitivity = savedValue;

        // 슬라이더 값 변경 → ScrollRect에 즉시 반영
        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
    }

    void OnApplicationQuit()
    {
        SaverLoader.SaveSettings(volumeSlider.value, sensitivitySlider.value);
    }

    public void Play(string name, float volume = 1f, bool isbgm = false)
    {
        audioSource.volume = volume;
        audioSource.loop = isbgm;
        if (sfxDict.ContainsKey(name))
        {
            if(isbgm)
            {
                audioSource.clip = sfxDict[name];
                audioSource.Play();
            }
            else audioSource.PlayOneShot(sfxDict[name], volume);
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
    /*
    public void playBGM(string name, float volume = 1f)
    {
        bgmSource.volume = volume;
        if (sfxDict.ContainsKey(name))
        {
            bgmSource.clip = sfxDict[name];
            bgmSource.Play();
        }
        else if (String.Equals(name, "endsound"))
        {
            bgmSource.Stop();
        }
        else
        {
            Debug.Log("unavailable audio name");
        }
    }
    */

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
        PlayerPrefs.SetFloat("Volume", volume);
    }
    
    public void SetSensitivity(float value)
    {
        logRect.scrollSensitivity = value;
        PlayerPrefs.SetFloat("LogScrollSensitivity", value); // 감도 저장
    }
}
