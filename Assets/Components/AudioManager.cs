using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private GameObject musicObject;
    [SerializeField] private GameObject SFXObject;
    private AudioSource musicSource;
    private AudioSource audioSource;
    private const int MINVOL = -80;
    private const int MAXVOL = 20;
    private float delay = 0.1f;
    private float timer = 0;
    public void SetMusicVolume(float volume)
    {
        mixer.SetFloat("MusicVolume", volume);
        if(timer>delay)
        {
            musicSource.Play();
            timer = 0;
        }else
        {
            timer += Time.deltaTime;
        }
    }

    public void SetSFXVolume(float volume)
    {
        mixer.SetFloat("SFXVolume", volume);
        audioSource.Play();
        if (timer > delay)
        {
            audioSource.Play();
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    private void Start()
    {
        musicSource = musicObject.GetComponent<AudioSource>();
        audioSource = SFXObject.GetComponent<AudioSource>();
    }



}
