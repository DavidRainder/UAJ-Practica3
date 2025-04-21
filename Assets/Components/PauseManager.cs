using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu = null;

    bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        isPaused = false;
        if(pauseMenu == null) { pauseMenu = transform.GetChild(0).gameObject; }
        pauseMenu.SetActive(isPaused);
    }

    // Update is called once per frame
    void Update()
    {
        // QUITAR ESTO POR INPUT MANAGER
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame(!isPaused);
        }
    }

    public void PauseGame(bool pauseGame)
    {
        isPaused = pauseGame;
        if (pauseMenu == null) Debug.LogWarning("Pause Menu has not been set. (UI Manager)");
        else
        {
            pauseMenu.SetActive(pauseGame);
        }
        if (isPaused)
        {
            GameManager.Instance.PauseGame();
        }
        else
        {
            GameManager.Instance.ResumeGame();
        }
    }

    public void BackToMenu()
    {
        GameManager.Instance.BackToMenu();
    }
}
