using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TelemetrySystem;

public class ToCreditsManin : MonoBehaviour
{
    private bool active = true;
    public void LoadFinalScene()
    {
        //Telemetry
        if(active) Tracker.Instance.PushEvent(new LevelEndEvent(GameManager.Instance.PLAY_NAME));
        active = false;

        GameManager.Instance.LoadScene("EndCinematic");

        this.enabled = false;
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
