using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class BackToMenu : MonoBehaviour
{
    public GameObject[] destroyObjects;
    public int menuScene;

    public void OnBackClick()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(menuScene);
        foreach(GameObject obj in destroyObjects)
        {
            Destroy(obj);
        }
        
    }
}
