using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

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
        levelSize = new Vector2Int(2*(int)levelBack.transform.localScale.x, 2*(int)levelBack.transform.localScale.y);
        levelGrid = new S_LevelManager(levelSize.x,levelSize.y, levelSize.x/marginPercent, levelSize.x/marginPercent, numberOfFood);
        refreshTimer = 0.0f;
        roomCache = GameObject.FindGameObjectWithTag("Room");
        if(roomCache != null)
        {
            playerCount = roomCache.GetComponent<PhotonRoom>().playersInRoom;
        }
        if (playerCount <= 0)
        {
            playerCount = 1;
        }
        if(levelTimer == 0)
        {
            levelTimer = 120.0f; //Two minute rounds
        }
        levelEnd = false;
        timerText.text = "Time: " + ((int)levelTimer).ToString();
    }

    void FixedUpdate()
    {
        // Only master client will handle level updates
        if(PhotonNetwork.IsMasterClient)
        {
            refreshTimer += Time.deltaTime;
            if (refreshTimer >= refreshRate)
            {
                // Refresh the level
                PV.RPC("RPC_RefreshLevel", RpcTarget.AllBuffered);
                refreshTimer -= refreshRate;
            }
            levelTimer -= Time.deltaTime;
            timerText.text = "Time: " + ((int)levelTimer).ToString();
            if (levelTimer <= 0.0f)
            {
                levelEnd = true;
                // RPC call for end level
            }
            else
            {
                timerText.text = "Time: " + ((int)levelTimer).ToString();
            }
        }
    }

    [PunRPC]
    void RPC_RefreshLevel()
    {
        levelGrid.RefreshLevel();
    }

    public Vector2Int GetWindowSize()
    {
        return levelSize;
    }

    public bool GetWarp()
    {
        return wrapAround;
    }

    //RPC call for round end
}
