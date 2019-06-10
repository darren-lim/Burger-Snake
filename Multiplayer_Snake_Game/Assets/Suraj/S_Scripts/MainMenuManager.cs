using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager: MonoBehaviour
{
    private int lobbyScene = 1;


    public void OnLobbyButtonClick()
    {
        SceneManager.LoadScene(lobbyScene);
    }
}
