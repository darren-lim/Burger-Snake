using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerSettings : MonoBehaviour
{
    public static MultiplayerSettings multiplayerSetting;
    
    public int maxPlayers;
    public int menuScene;
    public int multiplayerScene;

    private void Awake() 
    {
        if (MultiplayerSettings.multiplayerSetting == null)
        {
            MultiplayerSettings.multiplayerSetting = this;
        }
        else
        {
            if (MultiplayerSettings.multiplayerSetting != this)
            {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
