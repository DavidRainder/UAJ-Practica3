using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMenuSound : MonoBehaviour
{
    AudioSource lenguaSound;
    public void PlayLengua()
    {
        lenguaSound.Play();
    }
    // Start is called before the first frame update
    void Start()
    {
        lenguaSound = GetComponent<AudioSource>();
    }

}
