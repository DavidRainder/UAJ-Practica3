using System.Collections;
using System.Collections.Generic;
using TelemetrySystem;
using UnityEngine;

public class InteractionComponent : MonoBehaviour
{
    // Start is called before the first frame update
    enum type {Moss, Button};

    private bool check;
    private string playerName;
    [SerializeField]
    private type tipo;

    private List<KeyCode> allowedKeys;
    private bool checkMoss;
    private bool checkButton;

    void Start()
    {
        allowedKeys = new List<KeyCode>
        {
            KeyCode.W, KeyCode.A, KeyCode.D,
            KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.RightArrow
        };
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        MovementComponent player = collision.GetComponent<MovementComponent>();

        if(player!=null )
        {
            check = true;
            playerName = player.name;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        MovementComponent player = collision.GetComponent<MovementComponent>();

        if (player != null)
        {
            check = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (check)
        {
            if (Input.anyKey)
            {
                if (tipo == type.Moss)
                {
                    foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
                    {
                        if (Input.GetKey(key) && (!allowedKeys.Contains(key) && key != KeyCode.RightShift))
                        {
                            Tracker.Instance.PushEvent(new InteractionEvent(tipo.ToString(), false));
                            return; // salir, ya sabemos que fue fallo
                        }
                    }
                    if (Input.GetKey(KeyCode.RightShift)) // Si no hubo teclas prohibidas, ¿se pulsó Q?
                    {
                        if (playerName != "Bo")
                        {
                            Tracker.Instance.PushEvent(new InteractionEvent(tipo.ToString(), false));
                        }
                        else Tracker.Instance.PushEvent(new InteractionEvent(tipo.ToString(), true));
                    }
                }

                if (tipo == type.Button)
                {
                    foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
                    {
                        if (Input.GetKeyDown(key) && (!allowedKeys.Contains(key) && key != KeyCode.Q))
                        {
                            Tracker.Instance.PushEvent(new InteractionEvent(tipo.ToString(), false));
                            return; // salir, ya sabemos que fue fallo
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.Q)) // Si no hubo teclas prohibidas, ¿se pulsó Q?
                    {
                        if (playerName != "Marvin")
                        {
                            Tracker.Instance.PushEvent(new InteractionEvent(tipo.ToString(), false));
                        }
                        else Tracker.Instance.PushEvent(new InteractionEvent(tipo.ToString(), true));
                    }
                }
            }
        }
    }
}
