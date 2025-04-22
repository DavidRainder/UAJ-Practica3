using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using TelemetrySystem;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class ButtonEnviroment : MonoBehaviour
{
    [Tooltip("ID de ButtonEnviroment y de DoorEnviroment deben ser el mismo para que estas puertas estén linkadas.")]
    [SerializeField] int ID;

    Animator animator = null;

    Vector2 offset = new Vector2(0.0f, 0.32f);

    int numEntitiesColliding = 0;

    bool IsActive { get { return numEntitiesColliding > 0; } }

    DoorEnviroment linkedDoor = null;

    //Telemetry
    bool check = false;

    private void Awake()
    {
        gameObject.name = "Button_GO_" + ID;
    }

    private void Start()
    {
        string linkedDoorName = "Door_GO_" + ID;
        linkedDoor = GameObject.Find(linkedDoorName).GetComponent<DoorEnviroment>();

        var collider = GetComponent<BoxCollider2D>();
        collider.enabled = true;
        collider.isTrigger = true;
        collider.offset = offset;

        animator = GetComponent<Animator>();
    }

    private void PlayAnim(string anim)
    {
        animator.SetTrigger(anim);
    }

    private void Activate()
    {
        if (!IsActive)
        {
            PlayAnim("Activate");
            linkedDoor.Open();
        }
    }
        
    private void Deactivate()
    {
        if(!IsActive)
        {
            PlayAnim("Deactivate");
            linkedDoor.Close();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        MovementComponent player = collision.GetComponent<MovementComponent>();
        Tail tail = collision.GetComponent<Tail>();

        if(player != null || tail != null)
        {
            Activate();
            numEntitiesColliding++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        MovementComponent player = collision.GetComponent<MovementComponent>();
        Tail tail = collision.GetComponent<Tail>();

        if (player != null || tail != null)
        {
            numEntitiesColliding--;
            Deactivate();
            check = false;
        }
    }

    //Telemetry
    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    MovementComponent player = collision.GetComponent<MovementComponent>();

    //    if (player != null)
    //    {
    //        check = true;
    //    }
    //}

    ////Telemetry
    //private void Update()
    //{
    //    if (check)
    //    {
    //        if (Input.anyKeyDown)
    //        {
    //            bool isMovementKey =
    //                Input.GetKeyDown(KeyCode.UpArrow) ||
    //                Input.GetKeyDown(KeyCode.LeftArrow) ||
    //                Input.GetKeyDown(KeyCode.RightArrow) ||
    //                Input.GetKeyDown(KeyCode.W) ||
    //                Input.GetKeyDown(KeyCode.A) ||
    //                Input.GetKeyDown(KeyCode.D);

    //            if (Input.GetKeyDown(KeyCode.Q))
    //            {
    //                Tracker.Instance.PushEvent(new InteractionEvent("Button", true));
    //            }
    //            else if (!isMovementKey)
    //            {
    //                Tracker.Instance.PushEvent(new InteractionEvent("Button", false));
    //            }
    //        }
    //    }
    //}

}
