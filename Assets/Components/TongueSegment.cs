using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueSegment : MonoBehaviour
{
    [SerializeField]
    private GameObject connectedAbove, connectedBelow;
    // Start is called before the first frame update
    void Start()
    {
        var joint = GetComponent<HingeJoint2D>();
        connectedAbove = joint.connectedBody.gameObject;
        TongueSegment aboveSegment = connectedAbove.GetComponent<TongueSegment>();
        if (aboveSegment != null ) {
            aboveSegment.connectedBelow = gameObject;
            float spriteBottom = connectedAbove.GetComponent<SpriteRenderer>().bounds.size.y;
            joint.connectedAnchor = new Vector2(0, -spriteBottom);
        }
        else { 
            joint.connectedAnchor = new Vector2(0, 0);
        }
    }

}
