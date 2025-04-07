using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToCreditsManin : MonoBehaviour
{
    public void LoadFinalScene()
    {
        GameManager.Instance.LoadScene("EndCinematic");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerAnimationController uwu = collision.gameObject.GetComponent<PlayerAnimationController>();

        if(uwu != null)
        {
            LoadFinalScene();
        }
    }

}
