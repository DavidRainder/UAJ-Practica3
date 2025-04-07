using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGrabComponent : MonoBehaviour
{
    #region references
    Rigidbody2D body;
    PlayerAnimationController animation;
    #endregion

    bool grabInput;
    bool touchingGrass;
    bool hasLeft = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<MossComponent>() != null)
        {
            touchingGrass = true;
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<MossComponent>() != null)
        {
            touchingGrass = false;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        animation = GetComponent<PlayerAnimationController>();
        body = GetComponent<Rigidbody2D>();
    }
    public void Grab(bool input)
    {
        grabInput = input;
    }

    public bool IsGrabbing()
    {
        return hasLeft;
    }

    IEnumerator Impulse()
    {
        yield return new WaitForSeconds(0.08f);
        Vector2 force = gameObject.GetComponent<MovementComponent>().ImpulseNeeded();
        force = new Vector2(force.x, Mathf.Abs(force.y) + 1.2f) * new Vector2(0.7f, 0.7f);
        gameObject.GetComponent<Rigidbody2D>().AddForce(force);
        gameObject.GetComponent<MovementComponent>().OtherPlayer().GetComponent<Rigidbody2D>().AddForce(force * 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (grabInput && touchingGrass)
        {
            animation.BoGrab();   
            body.constraints = RigidbodyConstraints2D.FreezeAll;
            hasLeft = true;
        }
        else
        {
            animation.BoUnGrab();
            body.constraints = RigidbodyConstraints2D.FreezeRotation;
            if(hasLeft)
            {
                StartCoroutine(Impulse());
                hasLeft = false;
            }
        }
    }
}
