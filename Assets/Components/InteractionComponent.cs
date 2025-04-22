using System.Collections;
using System.Collections.Generic;
using TelemetrySystem;
using UnityEngine;

public class InteractionComponent : MonoBehaviour
{
    //Telemetry: Dada la complegidad del evento, y que el codigo original
    //fue realizado por 11 personas en una jam de 3 dias, por lo que la calidad
    //de este deja bastante que desear, este script ha sido creado enteramente
    //con fines de telemetricos (como excepcion)

    enum type {Moss, Button};

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        MovementComponent player = collision.GetComponent<MovementComponent>();

        if(player != null && tipo == type.Moss)
        {
            checkMoss = true;
            checkButton = false;
            playerName = player.name;
        }
        else if (player != null && tipo == type.Button)
        {
            checkButton = true;
            checkMoss = false;
            playerName = player.name;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        MovementComponent player = collision.GetComponent<MovementComponent>();

        if (player != null)
        {
            checkMoss = false;
            checkButton = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (checkMoss)
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
                            checkMoss = false;
                            return;
                        }
                    }
                    if (Input.GetKey(KeyCode.RightShift))
                    {
                        if (playerName != "Bo")
                        {
                            Tracker.Instance.PushEvent(new InteractionEvent(tipo.ToString(), false));

                        }
                        else Tracker.Instance.PushEvent(new InteractionEvent(tipo.ToString(), true));
                        checkMoss = false;
                    }
                }
            }
        }

        if (checkButton)
        {
            if (Input.anyKeyDown)
            {
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
