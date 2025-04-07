using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] string lizardName = "Marvin";

    Animator animator = null;
    Rigidbody2D rb = null;
    MovementComponent player = null;
    SpriteRenderer renderer;
    AudioSource fuckingDies;

    int lookingDir = 1;

    bool isJumping = false;

    private void Start()
    {
        animator = transform.GetChild(1).GetComponent<Animator>();
        renderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<MovementComponent>();
        fuckingDies = GetComponents<AudioSource>()[3];
    }

    private void Update()
    {
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * lookingDir, transform.localScale.y, transform.localScale.z);

        animator.SetFloat("VerticalSpeed", Mathf.Abs(rb.velocity.y));
    }

    public void StartJumping()
    {
        if(!isJumping) animator.SetBool("IsJumping", true);
        isJumping = true;
    }

    public void SetDirection(float dir)
    {
        animator.SetFloat("Speed", Mathf.Abs(dir));

        if (dir > 0) lookingDir = 1;
        else if (dir < 0) lookingDir = -1;
    }

    public void Die()
    {
        fuckingDies.Play();
        animator.SetTrigger("Dead");

    }

    public void Undie()
    {
        animator.SetBool("Dead", true);
    }

    public void Land()
    {
        animator.SetBool("IsJumping", false);
        isJumping = false;
    }

    public void BoGrab()
    {
        if(lizardName != "Marvin")
        {
            animator.SetBool("Grab", true);
        }
    }

    public void BoUnGrab()
    {
        if (lizardName != "Marvin")
        {
            animator.SetBool("Grab", false);
        }
    }

    public void ChangeAnimatorController(Animator controller) {
        animator = controller;
    }
}
