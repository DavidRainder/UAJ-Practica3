using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlatformComponent : MonoBehaviour
{
    [SerializeField]
    private Vector2 direction;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float distance;

    [SerializeField]
    private bool flipHorizontal = false;

    [SerializeField]
    private bool flipVertical = false;

    private HashSet<GameObject> objectsToMove;

    private SpriteRenderer sprite;

    private const int MAX_PLAYERS = 2;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        MovementComponent movement = collision.gameObject.GetComponent<MovementComponent>();
        if (movement != null && movement.gameObject.transform.position.y > transform.position.y)
        {
            
            objectsToMove.Add(collision.gameObject);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        MovementComponent movement = collision.gameObject.GetComponent<MovementComponent>();
        if (movement != null)
        {
            objectsToMove.Remove(collision.gameObject);
        }
    }

    private Vector3 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        objectsToMove = new HashSet<GameObject>(MAX_PLAYERS);
        initialPosition = transform.position;
        direction.Normalize();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movement2D = direction * speed * Time.deltaTime;
        Vector3 movement3D = new Vector3(movement2D.x, movement2D.y, 0);
        transform.position += movement3D;
        if(objectsToMove.Count > 0)
        {
            foreach(var movableObject in objectsToMove)
            {
                movableObject.transform.position += movement3D;
            }
        }
        if((transform.position - initialPosition).magnitude >= distance)
        {
            direction *= -1;
            initialPosition = transform.position;
            if(flipHorizontal)
            {
                sprite.flipX = !sprite.flipX;
            }
            if (flipVertical)
            {
                sprite.flipY = !sprite.flipY;
            }
        }
    }
}
