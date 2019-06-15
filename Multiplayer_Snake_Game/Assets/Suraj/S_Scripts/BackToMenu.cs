using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
    public GameObject[] destroyObjects;
    public int menuScene;

    public void OnBackClick()
    {
        SceneManager.LoadScene(menuScene);
        foreach(GameObject obj in destroyObjects)
        {
            Destroy(obj);
        }
        
    }
}
