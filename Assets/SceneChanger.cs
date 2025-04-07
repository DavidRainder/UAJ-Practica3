using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    public void ReloadPlayScene()
    {
        GameManager.Instance.LoadScene(GameManager.Instance.PLAY_NAME);
    }

    public void LoadPlayScene()
    {
        GameManager.Instance.LoadSceneNoFade(GameManager.Instance.PLAY_NAME);
    }

    public void LoadPlaySceneFade()
    {
        GameManager.Instance.LoadPlaySceneFade();
    }

    public void LoadCinematicFade()
    {
        GameManager.Instance.LoadCinematic(GameManager.Instance.CINEMATIC_NAME);
    }

    public void LoadCredits()
    {
        GameManager.Instance.LoadCredits();
    }

    public void LoadMenu()
    {
        GameManager.Instance.BackToMenu();
    }
}
