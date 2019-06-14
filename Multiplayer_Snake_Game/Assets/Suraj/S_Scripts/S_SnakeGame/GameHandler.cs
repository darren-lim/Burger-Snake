using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
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


    // Options for gameplay - CAN BE EXPANDED LATER
    private int numberOfFood = 20;
    private bool wrapAround = true;
    public bool selfCollision = true;

    // Network Stuff
    private GameObject roomCache;
    
    void Start()
    {
        levelBack = GameObject.FindGameObjectWithTag("Background");
        levelSize = new Vector2Int(2*(int)levelBack.transform.localScale.x, 2*(int)levelBack.transform.localScale.y);
        levelGrid = new S_LevelManager(levelSize.x,levelSize.y, levelSize.x/marginPercent, levelSize.x/marginPercent, numberOfFood);
        refreshTimer = 0.0f;
        roomCache = GameObject.FindGameObjectWithTag("Room");
        playerCount = roomCache.GetComponent<PhotonRoom>().playersInRoom;
        if (playerCount <= 0)
        {
            playerCount = 1;
        }
    }

    void FixedUpdate()
    {
        refreshTimer += Time.deltaTime;
        if (refreshTimer >= refreshRate)
        {
            // Refresh the level
            levelGrid.RefreshLevel();
            refreshTimer -= refreshRate;
        }
    }

    public Vector2Int GetWindowSize()
    {
        return levelSize;
    }

    public bool GetWarp()
    {
        return wrapAround;
    }
}
