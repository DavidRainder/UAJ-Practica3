using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;

public class BackgroundMusicController : MonoBehaviour
{
    AudioSource backgroundMusic;
    [SerializeField]
    float2 silenceRange;

    bool wasPlaying;

    float silenceTime;
    float timer;

    // Start is called before the first frame update
    void Start()
    {
        backgroundMusic = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (wasPlaying && !backgroundMusic.isPlaying)
        {
            silenceTime = UnityEngine.Random.Range(silenceRange.x, silenceRange.y);
            timer = 0;
        }
        if (!backgroundMusic.isPlaying)
        {
            if (timer < silenceTime)
            {
                timer += Time.deltaTime;
            }
            else
            {
                backgroundMusic.Play();
            }
        }
        wasPlaying = backgroundMusic.isPlaying;
    }
}
