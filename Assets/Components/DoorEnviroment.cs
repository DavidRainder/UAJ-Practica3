using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DoorEnviroment : MonoBehaviour
{
    [Tooltip("ID de ButtonEnviroment y de DoorEnviroment deben ser el mismo para que estas puertas estén linkadas.")]
    [SerializeField] int ID;

    BoxCollider2D collider = null;

    int numTonguesThroughDoor = 0;

    Animator animator;

    private void Awake()
    {
        gameObject.name = "Door_GO_" + ID;
    }

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        collider = GetComponent<BoxCollider2D>();
    }

    public void Open()
    {
        collider.isTrigger = true;
        animator.SetTrigger("Activate");
    }

    public void Close()
    {
        animator.SetTrigger("Deactivate");
        collider.isTrigger = false;

        if (numTonguesThroughDoor > 0)
        { 
            Debug.LogWarning("pierdeees");
            Debug.Log("LLamando a corrutina de que ela gente explote");
            GameManager.Instance.FreezePlayers(true);
            GameManager.Instance.ResetScene();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TongueComponent tongue = collision.transform.parent?.GetComponent<TongueComponent>();

        if(tongue != null)
        {
            Debug.LogWarning("AAAAAAAAAAAAAAAAAAA");
            numTonguesThroughDoor++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        TongueComponent tongue = collision.transform.parent?.GetComponent<TongueComponent>();

        if (tongue != null)
        {
            Debug.LogWarning("BBBBB");
            numTonguesThroughDoor--;
        }
    }
}
