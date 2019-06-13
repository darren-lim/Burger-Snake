using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerSettings : MonoBehaviour
{
    public static MultiplayerSettings multiplayerSetting;
    
    public int maxPlayers = 4;
    
    // Build order of scenes
    public int menuScene = 1;
    public int multiplayerScene = 2;

    private void Awake() 
    {
        // Setup Singleton
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
}
