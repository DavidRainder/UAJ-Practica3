using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TelemetrySystem;

public class ToCreditsManin : MonoBehaviour
{
    public void LoadFinalScene()
    {
        //Telemetry
        Tracker.Instance.PushEvent(new LevelEndEvent(GameManager.Instance.PLAY_NAME));

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
