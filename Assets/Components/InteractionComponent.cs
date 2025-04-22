using System.Collections;
using System.Collections.Generic;
using TelemetrySystem;
using UnityEngine;

public class InteractionComponent : MonoBehaviour
{
    // Start is called before the first frame update

    private bool check = false;
    private string playerName;
    void Start()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        MovementComponent player = collision.GetComponent<MovementComponent>();

        if(player!=null)
        {
            check = true;
            playerName = player.name;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (check)
        {
            if (Input.anyKey)
            {
               
                bool isMovementKey =
                    Input.GetKeyDown(KeyCode.UpArrow) ||
                    Input.GetKeyDown(KeyCode.LeftArrow) ||
                    Input.GetKeyDown(KeyCode.RightArrow) ||
                    Input.GetKeyDown(KeyCode.W) ||
                    Input.GetKeyDown(KeyCode.A) ||
                    Input.GetKeyDown(KeyCode.D);

                if(gameObject.GetComponent<MossComponent>())
                {
                    if (Input.GetKey(KeyCode.RightShift))
                    {
                        if(playerName != "Bo")
                        {
                           Tracker.Instance.PushEvent(new InteractionEvent("Moss", false));
                        }
                        Tracker.Instance.PushEvent(new InteractionEvent("Moss", true));
                    }
                    else if (!isMovementKey)
                    {
                        Tracker.Instance.PushEvent(new InteractionEvent("Moss", false));
                    }
                }

                if (gameObject.GetComponent<ButtonEnviroment>())
                {
                    if (Input.GetKeyDown(KeyCode.RightShift))
                    {
                        if (playerName != "Marvin")
                        {
                            Tracker.Instance.PushEvent(new InteractionEvent("Button", false));
                        }
                        Tracker.Instance.PushEvent(new InteractionEvent("Button", true));
                    }
                    else if (!isMovementKey)
                    {
                        Tracker.Instance.PushEvent(new InteractionEvent("Button", false));
                    }
                }
            }
        }
    }
}
