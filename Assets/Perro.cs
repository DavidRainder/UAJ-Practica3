using System.Collections;
using System.Collections.Generic;
using TelemetrySystem;
using UnityEngine;

public class Perro : MonoBehaviour
{
    float esperaTime = 5;
    float currentTime = 0;

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= esperaTime)
        {
            //Tracker.Instance.PushEvent(new GuauEvent());
            Destroy(gameObject);
        }
    }
}
