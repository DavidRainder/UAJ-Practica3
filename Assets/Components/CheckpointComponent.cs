using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointComponent : MonoBehaviour
{
    [SerializeField]
    private int id;

    private int numPlayers = 0;

    private GameObject lastObject = null;

    private static bool changePosition = true;

    public static void ChangePositionOnLoad(bool enable)
    {
        changePosition = enable;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        MovementComponent playerComp = collision.gameObject.GetComponent<MovementComponent>();
        // Si todavia quedan jugadores por pasar
        if(numPlayers < 2)
        {
            // Si el objeto colisionado tiene un componente de jugador
            if (playerComp != null && GameManager.Instance.getCurrentCheckpoint() < id)
            {
                // Si es distinto al anterior objeto que entró
                if (lastObject != collision.gameObject)
                {
                    numPlayers++;
                    lastObject = collision.gameObject;
                    // En caso de que hayan pasado todos, se añade
                    if(numPlayers == 2)
                    {
                        GameManager.Instance.setCurrentCheckpoint(id);
                    }
                }
            }
        }
    }

    void Awake()
    {
        GameManager.Instance.AddCheckpoint(this.gameObject, id);
    }

    // Start is called before the first frame update
    void Start()
    {
        if(changePosition)
        {
            GameManager.Instance.LoadPlayScene();
            changePosition = false;
        }
    }
}
