using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    public GameObject MainMenu;

    public GameObject TutorialPanel;

    public void OnStartBtnPress()
    {
        SceneManager.LoadScene(1);
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

    public void OnMenuBtnPress()
    {
        SceneManager.LoadScene(0);
    }

    public void OnBackBtnPress()
    {
        TutorialPanel.SetActive(false);

        MainMenu.SetActive(true);
    }
}
