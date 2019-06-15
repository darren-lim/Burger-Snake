using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviourPun
{
    // Level-focused Details
    private GameObject levelBack;
    private S_LevelManager levelGrid;
    private Vector2Int levelSize;
    private int marginPercent = 10;
    private float refreshRate = 10.0f;
    private float refreshTimer;

    // Player-focused Details
    [SerializeField]
    private int playerCount;

    //UI Stuff
    public Text timerText;
    public Text player1Points;
    public Text player2Points;
    public Text player3Points;
    public Text player4Points;

    public Text Winner;
    
    public Dictionary<string,int> playerScores = new Dictionary<string,int>();

    // Options for gameplay - CAN BE EXPANDED LATER
    private int numberOfFood = 20;
    private bool wrapAround = true;
    public bool selfCollision = true;
    private float levelTimer;
    private bool levelEnd;

    // Network Stuff
    private GameObject roomCache;
    private PhotonView PV;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        levelBack = GameObject.FindGameObjectWithTag("Background");
        levelSize = new Vector2Int(2 * (int)levelBack.transform.localScale.x, 2 * (int)levelBack.transform.localScale.y);
        levelGrid = gameObject.AddComponent<S_LevelManager>();
        levelGrid.StartManager(levelSize.x, levelSize.y, levelSize.x / marginPercent, levelSize.x / marginPercent, numberOfFood);
        //levelGrid = new S_LevelManager(levelSize.x,levelSize.y, levelSize.x/marginPercent, levelSize.x/marginPercent, numberOfFood);
        refreshTimer = 0.0f;
        roomCache = GameObject.FindGameObjectWithTag("Room");
        if (roomCache != null)
        {
            playerCount = roomCache.GetComponent<PhotonRoom>().playersInRoom;
        }
        if (playerCount <= 0)
        {
            playerCount = 1;
        }
        if (levelTimer == 0)
        {
            levelTimer = 20.0f; //Two minute rounds
        }
        levelEnd = false;
        timerText.text = "Time: " + ((int)levelTimer).ToString();
        playerScores.Add("player1Controller(Clone)", 0);
        playerScores.Add("player2Controller(Clone)", 0);
        playerScores.Add("player3Controller(Clone)", 0);
        playerScores.Add("player4Controller(Clone)", 0);
        try
        {
            player1Points.text = "Player 1 Score: " + playerScores["player1Controller(Clone)"].ToString();
            player2Points.text = "Player 2 Score: " + playerScores["player2Controller(Clone)"].ToString();
            player3Points.text = "Player 3 Score: " + playerScores["player3Controller(Clone)"].ToString();
            player4Points.text = "Player 4 Score: " + playerScores["player4Controller(Clone)"].ToString();
        }
        catch { };
        //Debug.Log(playerScores.Keys);
        Winner.enabled = false;
    }

    void FixedUpdate()
    {
        // Only master client will handle level updates
        if (PhotonNetwork.IsMasterClient)
        {
            refreshTimer += Time.deltaTime;
            if (refreshTimer >= refreshRate)
            {
                // Refresh the level
                PV.RPC("RPC_RefreshLevel", RpcTarget.AllBuffered);
                refreshTimer -= refreshRate;
            }
            PV.RPC("RPC_LevelTimer", RpcTarget.All);
            if (levelTimer <= 0.0f)
            {
                levelEnd = true;
                // RPC call for end level
                RestartGame();
            }
            // else
            // {
            //     timerText.text = "Time: " + ((int)levelTimer).ToString();
            // }
        }
    }

    public void UpdateScore(string name, int score)
    {
        PV.RPC("uScore",RpcTarget.All,name,score);
    }

    [PunRPC]
    void uScore(string name, int score)
    {
        playerScores[name] = score;
        try
        {
            player1Points.text = "Player 1 Score: " + playerScores["player1Controller(Clone)"].ToString();
            player2Points.text = "Player 2 Score: " + playerScores["player2Controller(Clone)"].ToString();
            player3Points.text = "Player 3 Score: " + playerScores["player3Controller(Clone)"].ToString();
            player4Points.text = "Player 4 Score: " + playerScores["player4Controller(Clone)"].ToString();
        }
        catch
        {
            Debug.Log("NotFound");
        }
    }

    [PunRPC]
    void RPC_RefreshLevel()
    {
        levelGrid.RefreshLevel();
    }

    [PunRPC]
    void UpdateTimer(float lvTimer)
    {
        timerText.text = "Time: " + ((int)lvTimer).ToString();
    }

    public Vector2Int GetWindowSize()
    {
        return levelSize;
    }

    public bool GetWarp()
    {
        return wrapAround;
    }

    [PunRPC]
    void RPC_LevelTimer()
    {
        if (levelTimer <= 0) return;
        levelTimer -= Time.deltaTime;
        timerText.text = "Time: " + ((int)levelTimer).ToString();
    }

    public void DisconnectPlayer()
    {
        StartCoroutine(DisconnectAndLoad());
    }

    IEnumerator DisconnectAndLoad()
    {
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom)
        {
            yield return null;
        }
        PhotonNetwork.Disconnect();
        Destroy(GameObject.Find("LobbyController"));
        Destroy(GameObject.Find("RoomController"));
        SceneManager.LoadScene(MultiplayerSettings.multiplayerSetting.MainMenu);
    }

    //RPC call for round end
    void RestartGame()
    {
        StartCoroutine(OnRestart());
    }
    IEnumerator OnRestart()
    {
        //add who won
        string playerstr = "player1Controller(Clone)";
        int playerscore = 0;
        foreach (var entry in playerScores)
        {
            if (playerscore <= entry.Value)
            {
                playerscore = entry.Value;
                playerstr = entry.Key;
            }
        }
        if (playerstr == "player1Controller(Clone)")
        {
            playerstr = "Player 1 Wins";
        }
        else if (playerstr == "player2Controller(Clone)")
        {
            playerstr = "Player 2 Wins";
        }
        else if (playerstr == "player3Controller(Clone)")
        {
            playerstr = "Player 3 Wins";
        }
        else if (playerstr == "player4Controller(Clone)")
        {
            playerstr = "Player 4 Wins";
        }
        Winner.text = playerstr;
        Winner.enabled = true;
        PV.RPC("ShowText", RpcTarget.All, playerstr);
        yield return new WaitForSeconds(4);
        //PhotonNetwork.DestroyAll();
        //PhotonNetwork.LoadLevel(2);
        /*
        PhotonNetwork.DestroyAll();
        Destroy(GameObject.Find("LobbyController"));
        Destroy(GameObject.Find("RoomController"));
        SceneManager.LoadScene(MultiplayerSettings.multiplayerSetting.menuScene);*/

        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom)
        {
            yield return null;
        }
        PhotonNetwork.Disconnect();
        Destroy(GameObject.Find("LobbyController"));
        Destroy(GameObject.Find("RoomController"));
        SceneManager.LoadScene(MultiplayerSettings.multiplayerSetting.MainMenu);
    }

    [PunRPC]
    void ShowText(string text)
    {
        Winner.text = text;
        Winner.enabled = true;
    }
}