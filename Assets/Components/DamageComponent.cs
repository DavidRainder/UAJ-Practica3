using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class DamageComponent : MonoBehaviour
{
    bool died = false;

    BoxCollider2D collider = null;

    // Tiempo para animaciones de muerte o lo que se quiera
    [SerializeField]
    private float DIE_TIME = 1.0f;
    private float actualTime = 0.0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        MovementComponent movement = collision.gameObject.GetComponent<MovementComponent>();
        if(movement != null)
        {
            died = true;
            GameManager.Instance.FreezePlayers(true);
            GameManager.Instance.HideTongue();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = true;
        actualTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (died)
        {
            actualTime += Time.deltaTime;
            if (actualTime >= DIE_TIME)
            {
                actualTime = 0.0f;
                GameManager.Instance.ResetScene();
                died = false;
            }
        }
    }
}
