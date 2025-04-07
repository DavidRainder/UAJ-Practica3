using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TailDropComponent : MonoBehaviour
{
    #region references
    [SerializeField]
    GameObject tailPrefab;
    [SerializeField]
    Transform tailDropPosition;

    [SerializeField]
    Animator tailAnimator;
    [SerializeField]
    Animator noTailAnimator;

    GameObject tail;
    #endregion

    #region variables
    float elapsedTime;
    bool tailInput;
    bool tailDropped;
    #endregion

    #region parameters
    [SerializeField]
    float cooldown = 1.5f;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        elapsedTime = cooldown;
        tailDropped = false;
    }

    public void TailInput(bool input)
    {
        tailInput = input;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if(tailInput && elapsedTime > cooldown)
        {
            elapsedTime = 0;
            if (tailDropped)
            {
                Destroy(tail);
                tailAnimator.gameObject.SetActive(true);
                noTailAnimator.gameObject.SetActive(false);
                GetComponent<PlayerAnimationController>().ChangeAnimatorController(tailAnimator);
                tailDropped = false;
            }
            else 
            {
                tail = Instantiate(tailPrefab, tailDropPosition.position, tailDropPosition.rotation);
                tailAnimator.gameObject.SetActive(false);
                noTailAnimator.gameObject.SetActive(true);
                GetComponent<PlayerAnimationController>().ChangeAnimatorController(noTailAnimator);
                tailDropped = true;
            }
        }
    }
}
