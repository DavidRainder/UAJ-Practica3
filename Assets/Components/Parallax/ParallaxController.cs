using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{

    public ParallaxLayer[] parallaxLayers;

    // Start is called before the first frame update
    void Start()
    {
        Camera camera = GetComponent<Camera>();

        foreach(ParallaxLayer layer in parallaxLayers)
        {
            layer.Initialize(camera);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(ParallaxLayer layer in parallaxLayers)
        {
            layer.Move(transform.position);
        }
    }
}
