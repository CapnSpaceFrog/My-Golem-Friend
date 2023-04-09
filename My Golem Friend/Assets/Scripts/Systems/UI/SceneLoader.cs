using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    public GameObject MainPanel;
    public GameObject TutorialPanel;

    private Animator anim;

    private void Awake()
    {
        Instance = this;

        anim = GetComponent<Animator>();
    }

    private IEnumerator LoadGameplayLevel(int sceneIndex)
    {
        anim.Play("SCENELOADER_FADEIN");
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        SceneManager.LoadScene(sceneIndex);
        yield return new WaitForSeconds(1.75f);
    }


    #region Message Receivers
    public void LoadMainMenu()
    {
        StartCoroutine(LoadGameplayLevel(0));
    }

    public void LoadGame()
    {
        StartCoroutine(LoadGameplayLevel(1));
    }

    public void LoadEndScene()
    {
        StartCoroutine(LoadGameplayLevel(2));
    }

    public void BackBtnPressed()
    {
        TutorialPanel.SetActive(false);

        MainPanel.SetActive(true);
    }

    public void TutorialbtnPressed()
    {
        MainPanel.SetActive(false);

        TutorialPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion
}
