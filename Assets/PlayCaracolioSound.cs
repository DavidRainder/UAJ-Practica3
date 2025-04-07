using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCaracolioSound : MonoBehaviour
{
    public void CaracolioSonand()
    {
        GetComponent<AudioSource>().Play();
    }

}
