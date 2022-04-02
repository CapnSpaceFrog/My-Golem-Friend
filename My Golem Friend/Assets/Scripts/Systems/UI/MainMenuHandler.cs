using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuHandler : MonoBehaviour
{
    public GameObject MainMenu;

    public GameObject TutorialPanel;

    public void OnStartBtnPress()
    {

    }

    public void OnTutorialBtnPress()
    {
        MainMenu.SetActive(false);

        TutorialPanel.SetActive(true);
    }

    public void OnQuitBtnPress()
    {
        Application.Quit();
    }

    public void OnBackBtnPress()
    {
        TutorialPanel.SetActive(false);

        MainMenu.SetActive(true);
    }
}
