using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] 
    GameObject marvin = null;
    [SerializeField]
    GameObject bo = null;

    MovementComponent movementMarvin;
    MovementComponent movementBo;
    WallGrabComponent wallGrab;
    TailDropComponent tailDrop;

    float marvinTime = 0.0f;
    float boTime = 0.0f;
    const float maxJumpTime = 0.5f;

    bool jumpMarvin = false;
    bool jumpBo = false;

    // Update is called once per frame

    void Start()
    {
        if (marvin == null) marvin = GameObject.Find("Marvin");
        if (bo == null) bo = GameObject.Find("Bo");

        movementMarvin = marvin.GetComponent<MovementComponent>();
        movementBo = bo.GetComponent<MovementComponent>();
        tailDrop = marvin.GetComponent<TailDropComponent>();
        wallGrab = bo.GetComponent<WallGrabComponent>();
    }
    void Update()
    {
        if (jumpMarvin)
        {
            marvinTime += Time.deltaTime;
        }
        else marvinTime = 0.0f;
        if (jumpBo)
        {
            boTime += Time.deltaTime;
        }
        else boTime = 0.0f;

        movementMarvin.SetDirection(Input.GetAxis("Horizontal"));
        movementBo.SetDirection(Input.GetAxis("Horizontal2"));

        movementMarvin.Jump(Input.GetKeyDown(KeyCode.W));
        movementBo.Jump(Input.GetKeyDown(KeyCode.UpArrow));

        jumpMarvin = Input.GetKey(KeyCode.W);
        jumpBo = Input.GetKey(KeyCode.UpArrow);

        if (jumpMarvin && marvinTime > maxJumpTime)
        {
            movementMarvin.ExtendJump(false);
        }
        else
        {
            movementMarvin.ExtendJump(jumpMarvin);
        }

        if (jumpBo && boTime > maxJumpTime)
        {
            movementBo.ExtendJump(false);
        }
        else
        {
            movementBo.ExtendJump(jumpBo);
        }

        wallGrab.Grab(Input.GetKey(KeyCode.RightShift));
        tailDrop.TailInput(Input.GetKeyDown(KeyCode.Q));
    }
}
