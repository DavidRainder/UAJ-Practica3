using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ToadComponent : MonoBehaviour
{
    bool jumpedOn = false;
    List<Collider2D> collidedPlayers = new List<Collider2D>();
    AudioSource sapoSonido;

    [SerializeField] private float force =  1.75f;
    bool checkPlayerCollided(Collider2D b)
    {
        bool found = false;
        foreach (Collider2D rb in collidedPlayers) {
            if (rb == b) found = true;
        }
        return found;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        MovementComponent player = collision.gameObject.GetComponent<MovementComponent>();
        if (player != null) {
            Rigidbody2D body = player.gameObject.GetComponent<Rigidbody2D>();
            Collider2D c = player.gameObject.GetComponent<Collider2D>();
            if (!checkPlayerCollided(c))
            {
                sapoSonido.Play();
                collidedPlayers.Add(c);
                body.AddForce(Vector2.up * force * player.getJumpForce());
                gameObject.GetComponent<Animator>().SetTrigger("jumpedOn");
                player.ResetAir();
                jumpedOn = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        MovementComponent player = collision.gameObject.GetComponent<MovementComponent>();
        if (player != null)
        {
            collidedPlayers.Remove(player.gameObject.GetComponent<Collider2D>());
        }
    }

    private void Start()
    {
        sapoSonido = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (jumpedOn) { 
            jumpedOn = false;
        }
    }
}
