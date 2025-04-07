using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TongueComponent : MonoBehaviour
{
    [SerializeField]
    private GameObject player1;
    [SerializeField]
    private GameObject player2;
    [SerializeField]
    private float player1Offset;
    [SerializeField]
    private float player2Offset;
    [SerializeField]
    private Transform enlongingTongue;
    private float spriteLength;

    private DistanceJoint2D joint;
    float maxDistance = 6;
    [SerializeField]
    float soundOffset = 0.05f;
    [SerializeField]
    float soundDeadZone = 0.3f;
    bool sounded = true;
    AudioSource stayMaxAudioSource;
    AudioSource getToMaxAudioSource;

    private void UpdateSound()
    {
        float distance = (player1.transform.position - player2.transform.position).magnitude;
        bool stayIsPlaying = stayMaxAudioSource.isPlaying;
        if (!stayIsPlaying && distance >= maxDistance - soundOffset)
        {
            stayMaxAudioSource.Play();
            getToMaxAudioSource.Play();
        }
        if (stayIsPlaying && distance < maxDistance - soundDeadZone)
        {
            stayMaxAudioSource.Stop();
        }
    }
    private void UpdateSprite()
    {
        Vector3 scale = enlongingTongue.localScale;
        Vector3 player1V = player1.transform.position + new Vector3(0, player1Offset, 0);
        Vector3 player2V = player2.transform.position + new Vector3(0, player2Offset, 0);
        Vector3 dir;
        dir = player1.transform.position - player2.transform.position;
        enlongingTongue.localScale = new Vector3(dir.magnitude / spriteLength * 25, scale.y, scale.z);
        transform.position = (player1V + player2V) / 2.0f;
        float angle = Vector3.AngleBetween(new Vector3(1, 0, 0), dir) / 2.0f;
        if (player1.transform.position.y < player2.transform.position.y)
        {
            angle *= -1;
        }
        transform.rotation = new Quaternion(0, 0, Mathf.Sin(angle), Mathf.Cos(angle));
    } 

    // Start is called before the first frame update
    void Start()
    {
        joint = player1.GetComponent<DistanceJoint2D>();
        maxDistance = joint.distance;
        spriteLength = enlongingTongue.GetComponent<SpriteRenderer>().size.x;
        var audio = GetComponents<AudioSource>();
        stayMaxAudioSource = audio[0];
        getToMaxAudioSource = audio[1];
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSprite();
        UpdateSound();
    }
}
