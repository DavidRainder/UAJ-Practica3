using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraComponent : MonoBehaviour
{
    [Serializable]
    struct Limits
    {
        public float left, right, top, bottom;
    }
    Camera cam;
    // Izquierda, Derecha, Abajo, Arriba
    [SerializeField]
    Limits cameraLimits;
    [SerializeField]
    GameObject target;
    [SerializeField]
    float lerpFactor;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = Vector3.Lerp(transform.position, target.transform.position, lerpFactor * Time.deltaTime);
        transform.position = new Vector3(Mathf.Clamp(newPos.x, cameraLimits.left, cameraLimits.right), Mathf.Clamp(newPos.y, cameraLimits.bottom, cameraLimits.top), transform.position.z);
    }
}
