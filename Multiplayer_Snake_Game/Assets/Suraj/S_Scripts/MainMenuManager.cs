using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager: MonoBehaviour
{
    private int lobbyScene = 1;
    public GameObject helpScreen; 


    public void OnLobbyButtonClick()
    {
        SceneManager.LoadScene(lobbyScene);
    }

    public void OnHelpButtonClick()
    {
        helpScreen.SetActive(true);
    }

    public void OnCloseHelpButtonClick()
    {
        helpScreen.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
