using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovableObject : MonoBehaviour
{
    Rigidbody2D rb;

    bool isPaused = false;

    struct RB_Info
    {
        public Vector3 velocity;
        public float angularVelocity;
        public RigidbodyConstraints2D constraints;
    }

    RB_Info objectInfo;

    // Start is called before the first frame update
    void Start()
    {
        isPaused = false;
        GameManager.Instance.AddMovableObject(this);

        rb = GetComponent<Rigidbody2D>();
    }

    public void Pause()
    {
        if (isPaused) return;

        isPaused = true;

        objectInfo.velocity = rb.velocity;
        objectInfo.angularVelocity = rb.angularVelocity;
        objectInfo.constraints = rb.constraints;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0.0f;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void Resume()
    {
        if (!isPaused) return;

        isPaused = false;

        rb.velocity = objectInfo.velocity;
        rb.angularDrag = objectInfo.angularVelocity;
        rb.constraints = objectInfo.constraints;

    }
}
