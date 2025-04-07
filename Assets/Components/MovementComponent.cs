using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerAnimationController))]
public class MovementComponent : MonoBehaviour
{
    #region references
    Rigidbody2D body;
    PlayerAnimationController animation;
    AudioSource stepAudioSource;
    AudioSource groundAudioSource;
    AudioSource jumpAudioSource;
    [SerializeField]
    MovementComponent otherMovement;
    WallGrabComponent grabComp;
    #endregion

    #region variables
    float direction;
    bool jumpInput;
    bool jumpExtend;
    bool movementEnabled = true;
    bool airMovement = false;
    bool isGrounded = false;
    bool isFreezed = false;
    bool wasGrounded = false;
    #endregion

    #region parameters
    [SerializeField]
    float speed = 15;
    [SerializeField]
    float impulseSpeed = 8;
    [SerializeField]
    float jumpForce = 1000;
    [SerializeField]
    float gravityScale = 5f;
    [SerializeField]
    float stepSoundDeadZone = 0.005f;
    [SerializeField]
    Vector2 maxVel;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = gravityScale;
        
        animation = GetComponent<PlayerAnimationController>();

        var audios = GetComponents<AudioSource>();
        stepAudioSource = audios[0];
        groundAudioSource = audios[1];
        jumpAudioSource = audios[2];
        grabComp = otherMovement.gameObject.GetComponent<WallGrabComponent>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnvironmentComponent collider = collision.GetComponent<EnvironmentComponent>();
        if(collider != null)
        {
            //Debug.Log("ESTOY GROUNDED");
            animation.Land();
            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        EnvironmentComponent collider = collision.GetComponent<EnvironmentComponent>();
        if(collider != null)
        {
            //Debug.Log("YA NO ESTOY GROUNDED");
            isGrounded = false;
        }
    }

    public bool IsGrounded() { return isGrounded; }
    public bool CanAirMove() { return airMovement; }

    public void SetDirection(float movement)
    { 
        direction = movement;

        animation.SetDirection(direction);
      
    }

    public void Jump(bool input) {
        jumpInput = input;
    }

    public void ExtendJump(bool input) {
        jumpExtend = input;
        if(jumpExtend) animation.StartJumping();
    }

    public float getJumpForce()
    {
        return jumpForce;
    }

    public Vector2 getVel()
    {
        return body.velocity;
    }

    public void FreezePlayer(bool enable)
    {
        if(enable && !isFreezed)
        {
            body.velocity = new Vector2(0, 0);
            body.angularVelocity = 0;
            // body.constraints = RigidbodyConstraints2D.FreezeAll;
            body.gravityScale = 0;
            body.bodyType = RigidbodyType2D.Static;
            movementEnabled = false;
            animation.Die();
        }
        else if(!enable && isFreezed)
        {
            body.gravityScale = gravityScale;
            movementEnabled = true;
            body.bodyType = RigidbodyType2D.Dynamic;
            // body.constraints = RigidbodyConstraints2D.None;
            animation.Undie();
        }
        isFreezed = enable;
    }
    private void UpdateGroundSound()
    {
        if (isGrounded && !wasGrounded)
        {
            groundAudioSource.Play();
        }
        wasGrounded = isGrounded;
    }
    private void UpdateStepSound()
    {
        if (isGrounded && Mathf.Abs(body.velocity.x) > stepSoundDeadZone && !stepAudioSource.isPlaying)
        {
            stepAudioSource.Play();
        }
    }

    public GameObject OtherPlayer()
    {
        return otherMovement.gameObject;
    }

    IEnumerator NextJump()
    {
        yield return new WaitForSeconds(0.05f);
        Vector2 impulse = (otherMovement.getVel() + new Vector2(0, 1.0f)) * impulseSpeed * 15.0f;
        body.AddForce(impulse);
    }

    public Vector2 ImpulseNeeded()
    {
        if(!otherMovement.IsGrounded() && !otherMovement.CanAirMove())
            return (otherMovement.getVel() + new Vector2(2.0f, 0.0f)) * impulseSpeed * 15.0f;
        return new Vector2(0, 0);
    }

    public void ResetAir()
    {
        airMovement = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (movementEnabled)
        {
            if (!isGrounded)
            {
                if (airMovement)
                {
                    body.velocity = new Vector2(speed * direction, body.velocity.y);
                    if(body.velocity.y < 0) airMovement = false;
                }
                else if(otherMovement.IsGrounded() || (grabComp != null && grabComp.IsGrabbing()))
                {
                    body.AddForce(new Vector2(direction * impulseSpeed, impulseSpeed));
                }
                body.gravityScale = (jumpExtend & body.velocity.y > 0 ? gravityScale/2f : gravityScale);
            }
            else {
                body.velocity = new Vector2(speed * direction, body.velocity.y);
                if (jumpInput)
                {
                    if(!otherMovement.IsGrounded() && !otherMovement.CanAirMove())
                    {
                        StartCoroutine(NextJump());
                        Vector2 impulse = (otherMovement.getVel() + new Vector2(0, 1.0f)) * impulseSpeed * 15.0f;
                        otherMovement.gameObject.GetComponent<Rigidbody2D>().AddForce(impulse/1.5f);
                    }
                    else
                    {
                        body.AddForce(Vector2.up * jumpForce);
                    }   
                    airMovement = true;
                    jumpAudioSource.Play();
                }
            }
            body.velocity = new Vector2(Mathf.Clamp(body.velocity.x, -maxVel.x, maxVel.x), Mathf.Clamp(body.velocity.y, -maxVel.y, maxVel.y));
        }
        UpdateStepSound();
        UpdateGroundSound();
    }
}


