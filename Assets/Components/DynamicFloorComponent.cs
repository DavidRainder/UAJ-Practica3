using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicFloorComponent : MonoBehaviour
{
    Collider2D _myCollider;
    Collider2D pies;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == pies)
        {
            _myCollider.isTrigger = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == pies) { 
            _myCollider.isTrigger = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerAnimationController>() != null)
        {
            _myCollider.isTrigger = true;
        }
    }

    private void Start()
    {
        _myCollider = gameObject.GetComponent<Collider2D>();
        if (gameObject.layer == 6) pies = GameObject.Find("PiesMarvin").GetComponent<Collider2D>();
        else pies = GameObject.Find("PiesBo").GetComponent<Collider2D>();
    }
}
