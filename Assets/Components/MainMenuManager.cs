using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TelemetrySystem;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject settingsMenu = null;
    [SerializeField] GameObject mainMenu = null;
    [SerializeField] Button continueButton = null;

    [SerializeField] Animator menuAnimator = null;
    [SerializeField] Animator leafAnimator = null;

    private void Start()
    {
        ShowSettings(false);

        if(continueButton == null)
        {
            continueButton = GameObject.Find("Continue_Button").GetComponent<Button>();
        }
        continueButton.interactable = GameManager.Instance.SavedGame;
    }

    public void LoadGame()
    {
        menuAnimator.SetTrigger("Start");
        leafAnimator.SetTrigger("Start");
    }

    private void _ContinueGame()
    {
        menuAnimator.SetTrigger("Continue");
        leafAnimator.SetTrigger("Start");
    }

    public void ContinueGame()
    {
        _ContinueGame();
        GameManager.Instance.GameWasContinued();
    }

    public void ExitGame()
    {
        GameManager.Instance.ExitApplication();
    }

    public void ShowSettings(bool show)
    {
        mainMenu.SetActive(!show);
        settingsMenu.SetActive(show);
    }
}
