using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    float timeToDrown = 3.0f;
    [SerializeField]
    float drownHeight = 5.0f;

    bool isDrowning = false;
    float drownPerTick = 0.0f;
    float initialHeight = 0.0f;
    Transform _myTransform;

    List<Rigidbody2D> players = new List<Rigidbody2D>();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<MovementComponent>() != null)
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (!players.Contains(rb)) players.Add(rb);
            isDrowning = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<MovementComponent>() != null)
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (players.Contains(rb)) players.Remove(rb);
            if (players.Count == 0) isDrowning = false;
        }
    }

    void Start()
    {
        _myTransform = gameObject.GetComponent<Transform>();
        drownPerTick = drownHeight / timeToDrown * 0.01f;
        initialHeight = _myTransform.position.y;
    }

    private void FixedUpdate()
    {
        if (isDrowning && _myTransform.position.y >= initialHeight - drownHeight)
        {
            _myTransform.position -= new Vector3(0, drownPerTick, 0);
            foreach (Rigidbody2D rb in players)
            {
                rb.position -= new Vector2(0, drownPerTick);
            }
        }

        if (_myTransform.position.y < initialHeight && !isDrowning) 
            _myTransform.position += new Vector3(0, drownPerTick, 0);
    }
}
