using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicaEpica : MonoBehaviour
{
    public void PlayMusic()
    {
        GetComponent<AudioSource>().Play();
    }

}
